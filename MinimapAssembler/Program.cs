using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

var extractPath = "C:\\FreedomWoW\\Minimaps";

var outputPath = "C:\\FreedomWoW\\MinimapsAssembled";

Console.WriteLine($"Using extract path: {extractPath}");
Console.WriteLine($"Using output path: {outputPath}");

var maps = Directory.GetDirectories(extractPath).Select(Path.GetFileName);

var outputImgSize = 512;


foreach(var map in maps)
{
    Console.WriteLine($"Processing {map}...");

    var mapExtractPath = Path.Combine(extractPath, map);
    var firstFile = Directory.EnumerateFiles(mapExtractPath).FirstOrDefault();
    if (string.IsNullOrEmpty(firstFile))
    {
        Console.WriteLine($"No files found for map {map}, skipping.");
        continue;
    }

    int tileSize = 512;

    using (var firstImg = Image.Load(firstFile))
    {
        tileSize = firstImg.Width;
    }

    var mapOutputPath = Path.Join(outputPath, map);
    if (!Path.Exists(mapOutputPath))
    {
        Directory.CreateDirectory(mapOutputPath);
    }


    for (var level = 0; level < 7; level++)
    {
        Console.WriteLine($"Processing level {level}...");
        var sliceSize = (int) Math.Pow(2, level);
        var levelOutputPath = Path.Combine(mapOutputPath, level.ToString());
        if (!Path.Exists(levelOutputPath))
        {
            Directory.CreateDirectory(levelOutputPath);
        }

        for (var x = 0; x < (64 / sliceSize); x++)
        {
            for (var y = 0; y < (64 / sliceSize); y++)
            {
                using var sliceImg = new Image<Rgba32>(tileSize * sliceSize, tileSize * sliceSize, new Rgba32(0, 0, 0, 1));
                var hasImgData = false;
                for (var sliceX = 0; sliceX < sliceSize; sliceX++)
                {
                    var tileX = (x * sliceSize) + sliceX;
                    for (var sliceY = 0; sliceY < sliceSize; sliceY++)
                    {
                        var tileY = (y * sliceSize) + sliceY;

                        var file = Path.Combine(mapExtractPath, $"{tileX}_{tileY}.png");
                        if (File.Exists(file))
                        {
                            hasImgData = true;
                            using var tileImg = Image.Load(file);
                            sliceImg.Mutate(o => o.DrawImage(tileImg, new Point(sliceY * tileSize, sliceX * tileSize), 1f));
                        }
                    }
                }
                if (hasImgData)
                {
                    var outputFileName = Path.Combine(levelOutputPath, $"{x}_{y}.png");
                    sliceImg.Mutate(o => o.Resize(outputImgSize, outputImgSize));
                    sliceImg.SaveAsPng(outputFileName);
                }
            }
        }
        Console.WriteLine($"Level {level} processed.");
    }

    Console.WriteLine($"Map {map} processed.");
}
