using MyWebApi.Services.Interfaces;

namespace MyWebApi.Services.Implementations
{
    public class FileService(IWebHostEnvironment env) : IFileService
    {
        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            const long MaxFileSize = 30 * 1024 * 1024;
            if (file.Length > MaxFileSize)
                throw new Exception("The file is too large! Please choose a file under 30MB");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                throw new Exception("File format not supported!");

            var uploadsFolder = Path.Combine(env.WebRootPath, folderName);
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
        
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/{folderName}/{fileName}";
        }

    }
}
