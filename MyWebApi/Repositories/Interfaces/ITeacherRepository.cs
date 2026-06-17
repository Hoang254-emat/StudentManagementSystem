using MyWebApi.Entities;

namespace MyWebApi.Repositories.Interfaces
{
    public interface ITeacherRepository
    {
        Task<IEnumerable<Teacher>> GetAllAsync(string? keyword, string? sort, int page, int pageSize);
        Task<int> GetTotalCountAsync(string? keyword);
        Task<Teacher?> GetById(string id);
        Task AddAsync(Teacher teacher);
        Task UpdateAsync(Teacher teacher);
        Task DeleteAsync(Teacher teacher);
    }
}
