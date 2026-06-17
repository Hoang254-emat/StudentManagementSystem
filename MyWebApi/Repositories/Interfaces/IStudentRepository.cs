using MyWebApi.Entities;

namespace MyWebApi.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllStudents(string? keyword, string? sort, int page, int pageSize);
        Task<int> GetTotalCountAsync(string? keyword);
        Task<Student?> GetStudentById(string id);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(Student student);
        Task<bool> StudentExists(string id);
    }
}
