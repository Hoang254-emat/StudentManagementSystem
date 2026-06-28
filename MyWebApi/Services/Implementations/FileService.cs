using MyWebApi.Services.Interfaces;

namespace MyWebApi.Services.Implementations
{
    public class FileService(IWebHostEnvironment env) : IFileService
    {
        public async Task<(bool Success, string Message, string? Url)> UploadFileAsync(IFormFile file, string folderName)
        {
            const long MaxFileSize = 30 * 1024 * 1024;
            if (file.Length > MaxFileSize)
                return (false, "File quá lớn! Vui lòng chọn file dưới 30MB", null);

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return (false, "Định dạng file không được hỗ trợ!", null);

            try
            {
                var uploadsFolder = Path.Combine(env.WebRootPath, folderName);
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                return (true, "Thành công", $"/{folderName}/{fileName}");
            }
            catch (Exception ex)
            {
                // log
                return (false, $"Lỗi hệ thống: {ex.Message}", null);
            }
        }
        public async Task DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            var relativePath = fileUrl.TrimStart('/');
            var filePath = Path.Combine(env.WebRootPath, relativePath);

            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }
    }
}
