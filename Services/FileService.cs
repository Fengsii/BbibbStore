using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;

namespace EFENGSI_RAHMANTO_ZALUKHU.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public FileService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<string> SaveFileAsync(IFormFile file, string subDirectory)
        {
            if (file == null || file.Length == 0)
                return null;

            // Buat direktori jika belum ada
            var uploads = Path.Combine(_environment.WebRootPath, "uploads", subDirectory);
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            // Generate nama file unik
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine("uploads", subDirectory, fileName).Replace("\\", "/");
        }

        public async Task DeleteFileAsync(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                var fullPath = Path.Combine(_environment.WebRootPath, filePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
        }

        public string GetFileUrl(string fileName, string subDirectory)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            var request = _httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}/uploads/{subDirectory}/{fileName}";
        }





    }

}
