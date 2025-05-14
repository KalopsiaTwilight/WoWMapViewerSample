using CASCLib;

namespace MinimapExtractor.Providers
{
    public class CASCFileDataProvider : IFileDataProvider
    {
        private readonly CASCHandler cascHandler;

        public CASCFileDataProvider(string basePath)
        {
            CASCConfig.LoadFlags = LoadFlags.Install;
            CASCConfig.ValidateData = false;
            CASCConfig.ThrowOnFileNotFound = false;
            CASCConfig.ThrowOnMissingDecryptionKey = false;
            CASCConfig.UseOnlineFallbackForMissingFiles = false;
            CASCConfig.UseWowTVFS = false;

            var cascConfig = CASCConfig.LoadLocalStorageConfig(basePath, "wow");

            cascHandler = CASCHandler.OpenStorage(cascConfig);
            cascHandler.Root.SetFlags(LocaleFlags.enUS);
        }

        public bool FileIdExists(uint fileDataId)
        {
            return cascHandler.FileExists((int)fileDataId);
        }

        public Stream GetFileById(uint filedataId)
        {
            return cascHandler.OpenFile((int)filedataId);
        }
    }
}
