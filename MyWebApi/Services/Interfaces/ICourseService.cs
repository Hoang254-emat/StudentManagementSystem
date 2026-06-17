using MyWebApi.DTOs;
using MyWebApi.Helpers;

namespace MyWebApi.Services.Interfaces
{
    public interface ICourseService
    {
        Task<PagedResponse<CourseDto>> GetAllCoursesAsync(string? keyword, string? sort, int page, int pageSize);
        Task<CourseDto?> GetCourseByIdAsync(int id);
        Task<CourseDto> CreateCourseAsync(CreateCourseDto dto);
        Task<bool> UpdateCourseAsync(int id, CreateCourseDto dto);
        Task<bool> DeleteCourseAsync(int id);
        Task<bool> UpdateCurriculumUrlAsync(int id, string curriculumUrl);
    }
}
