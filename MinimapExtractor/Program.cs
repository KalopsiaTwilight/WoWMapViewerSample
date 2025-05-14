using BLPSharp;
using DBCD.Providers;
using MinimapExtractor;
using MinimapExtractor.Providers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

// TODO: Parametrize
var cascBasePath = "C:\\FreedomWoW\\Client";
var outputPath = "C:\\FreedomWoW\\Minimaps";

Console.WriteLine($"Extracting minimaps from CASC path: {cascBasePath}");
Console.WriteLine($"Writing files to output path: ${outputPath}");

var fileDataProvider = new CASCFileDataProvider(cascBasePath);

var dbcProvider = new FileDataDBCProvider(fileDataProvider);
var dbdProvider = new GithubDBDProvider();

var dbcd = new DBCD.DBCD(dbcProvider, dbdProvider);

var mapEntries = dbcd.Load("Map");

if (!Path.Exists(outputPath))
{
    Directory.CreateDirectory(outputPath);
}

foreach(var record in mapEntries.Values)
{
    var mapName = record["MapName_lang"] as string;
    if(mapName == null)
    {
        mapName = "Unknown_map_" + record.ID;   
    }

    var wdtFileId = (int) record["WdtFileDataID"];

    Console.WriteLine($"Processing map {record.ID} - {mapName}");

    List<string> illegalChars = ["<", ">", "\\", "|", ":", "/", "?", "\"", "*"];

    foreach (var illegalChar in illegalChars)
    {
        mapName = mapName.Replace(illegalChar, "");
    }

    var mapOutputPath = Path.Join(outputPath, $"{record.ID} - {mapName}");
    if (!Path.Exists(mapOutputPath))
    {
        Directory.CreateDirectory(mapOutputPath);
    }

    if (wdtFileId == 0)
    {
        Console.WriteLine("No wdt file linked, skipping...");
        // Skip file for now;
        continue;
    }

    var wdtInfo = fileDataProvider.GetFileById((uint) wdtFileId);
    if (wdtInfo == null)
    {
        Console.WriteLine("Wdt linked is not found in client data, skipping...");
        continue;
    }
    var minimapFileIds = WdtReader.ReadMinimapFileIds(wdtInfo);

    foreach (var minimapFile in minimapFileIds)
    {
        var blpData = fileDataProvider.GetFileById(minimapFile.Value);
        if (blpData == null)
        {
            continue;
        }
        var blp = new BLPFile(blpData);
        var imgData = blp.GetPixels(0, out var w, out var h);
        var img = Image.LoadPixelData<Bgra32>(imgData, w, h);
        img.SaveAsPng(Path.Join(mapOutputPath, minimapFile.Key + ".png"));
    }

    Console.WriteLine($"Map {record.ID} - {mapName} processed.");
}