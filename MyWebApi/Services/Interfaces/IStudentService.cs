using MyWebApi.DTOs;
using MyWebApi.Helpers;

namespace MyWebApi.Services.Interfaces
{
    public interface IStudentService
    {
        public Task<PagedResponse<StudentDto>> GetAllStudentsAsync(string? keyword, string? sort, int page, int pageSize);
        public Task<StudentDto?> GetStudentByIdAsync(string id);
        public Task<bool> CreateStudentAsync(CreateStudentDto createDto);
        public Task<bool> UpdateStudentAsync(string id, UpdateStudentDto updateDto);
        public Task<bool> DeleteStudentAsync(string id);
        Task<bool> UpdateAvatarUrlAsync(string id, string avatarUrl);
    }
}
