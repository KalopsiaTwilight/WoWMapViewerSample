namespace MinimapExtractor
{
    internal class WdtReader
    {
        public static Dictionary<string, uint> ReadMinimapFileIds(Stream data)
        {
            var minimapFileIds = new Dictionary<string, uint>();
            using (var bin = new BinaryReader(data))
            {
                long position = 0;
                while (position < data.Length)
                {
                    data.Position = position;

                    var chunkName = bin.ReadUInt32();
                    var chunkSize = bin.ReadUInt32();

                    position = data.Position + chunkSize;

                    switch (chunkName)
                    {
                        case 0x4D414944:
                            for (var x = 0; x < 64; x++)
                            {
                                for (var y = 0; y < 64; y++)
                                {
                                    bin.ReadBytes(28);
                                    var minimapFDID = bin.ReadUInt32();

                                    if (minimapFDID != 0)
                                    {
                                        minimapFileIds.Add($"{x}_{y}", minimapFDID);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            return minimapFileIds;
        }
    }
}
