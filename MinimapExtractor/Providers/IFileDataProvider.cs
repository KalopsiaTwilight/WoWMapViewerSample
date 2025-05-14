namespace MinimapExtractor.Providers
{
    public interface IFileDataProvider
    {
        public bool FileIdExists(uint fileDataId);
        public Stream GetFileById(uint filedataId);
    }
}
