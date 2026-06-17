using MyWebApi.Entities;

namespace MyWebApi.Repositories.Interfaces
{
    public interface IClassRepository
    {
        Task<IEnumerable<Class>> GetAllAsync(string? keyword, string? sort, int page, int pageSize);
        Task<int> GetTotalCountAsync(string? keyword);
        Task<Class?> GetByIdAsync(int id);
        Task AddAsync(Class @Class);
        Task UpdateAsync(Class @Class);
        Task DeleteAsync(Class @Class);
    }
}
