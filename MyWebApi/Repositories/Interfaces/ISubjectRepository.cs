using MyWebApi.Entities;

namespace MyWebApi.Repositories.Interfaces
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetAllAsync(string? keyword, string? sort, int page, int pageSize);
        Task<int> GetTotalCountAsync(string? keyword);
        Task AddAsync(Subject subject);
        Task<bool> ExistsAsync(string id);
        Task<Subject?> GetByIdAsync(string id);
        Task UpdateAsync(Subject subject);
        Task DeleteAsync(Subject subject);
    }
}
