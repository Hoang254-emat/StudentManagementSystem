using MyWebApi.DTOs;
using MyWebApi.Helpers;

namespace MyWebApi.Services.Interfaces
{
    public interface IClassService
    {
        Task<PagedResponse<ClassDto>> GetAllClassesAsync(string? keyword, string? sort, int page, int pageSize);
        Task<ClassDto?> GetClassByIdAsync(int id);
        Task<ClassDto?> CreateClassAsync(CreateClassDto createDto);
        Task<bool> UpdateClassAsync(int id, CreateClassDto updateDto);
        Task<bool> DeleteClassAsync(int id);
    }
}
