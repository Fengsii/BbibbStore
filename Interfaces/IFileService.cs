namespace EFENGSI_RAHMANTO_ZALUKHU.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string subDirectory);
        Task DeleteFileAsync(string filePath);
        string GetFileUrl(string fileName, string subDirectory);
    }
}
