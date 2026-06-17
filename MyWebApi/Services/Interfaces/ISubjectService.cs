using MyWebApi.DTOs;
using MyWebApi.Helpers;

namespace MyWebApi.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<PagedResponse<SubjectDto>> GetAllSubjectsAsync(string? keyword, string? sort, int page, int pageSize);
        Task<bool> CreateSubjectAsync(CreateSubjectDto createDto);
        Task<bool> UpdateSubjectAsync(string id, SubjectDto updateDto);
        Task<bool> DeleteSubjectAsync(string id);
    }
}
