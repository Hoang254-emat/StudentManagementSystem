using MyWebApi.DTOs;
using MyWebApi.Helpers;

namespace MyWebApi.Services.Interfaces
{
    public interface ITeacherService
    {
        Task<PagedResponse<TeacherDto>> GetAllTeachersAsync(string? keyword, string? sort, int page, int pageSize);
        Task<bool> CreateTeacherAsync(CreateTeacherDto createDto);
        public Task<TeacherDto?> GetByIdAsync(string id);
        Task<bool> UpdateTeacherAsync(string id, TeacherDto updateDto);
        Task<bool> DeleteTeacherAsync(string id);
        Task<bool> UpdateAvatarUrlAsync(string id, string avatarUrl);
    }
}
