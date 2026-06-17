using MyWebApi.Entities;

namespace MyWebApi.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllAsync(string? keyword, string? sort, int page, int pageSize);
        Task<int> GetTotalCountAsync(string? keyword);
        Task<Course?> GetByIdAsync(int id);
        Task AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(Course course);
    }
}
