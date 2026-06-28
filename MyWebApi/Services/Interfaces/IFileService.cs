namespace MyWebApi.Services.Interfaces
{
    public interface IFileService
    {
        Task<(bool Success, string Message, string? Url)> UploadFileAsync(IFormFile file, string folderName);
        Task DeleteFileAsync(string fileUrl);
    }
}
