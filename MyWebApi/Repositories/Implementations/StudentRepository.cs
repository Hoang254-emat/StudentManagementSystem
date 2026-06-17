using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Entities;
using MyWebApi.Repositories.Interfaces;

namespace MyWebApi.Repositories.Implementations
{
    public class StudentRepository(DataContext context) : IStudentRepository
    {
        private readonly DataContext _context = context;

        public async Task<IEnumerable<Student>> GetAllStudents(string? keyword, string? sort, int page, int pageSize)
        {
            var query = _context.Students
                .Include(s => s.Class)
                .AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => s.Name.Contains(keyword) || s.Id.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(sort))
            {
                if (sort.Equals("name", StringComparison.CurrentCultureIgnoreCase))
                {
                    query = query.OrderBy(s => s.Name);
                }
                else if (sort.Equals("id", StringComparison.CurrentCultureIgnoreCase))
                {
                    query = query.OrderBy(s => s.Id);
                }
            }
            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? keyword)
        {
            var query = _context.Students.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => s.Name.Contains(keyword) || s.Id.Contains(keyword));
            }
            return await query.CountAsync();
        }

        public async Task<Student?> GetStudentById(string id)
        {
            return await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Student student)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> StudentExists(string id)
        {
            return await _context.Students.AnyAsync(s => s.Id == id);
        }
    }
}
