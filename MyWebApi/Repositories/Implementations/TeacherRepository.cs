using MyWebApi.Entities;
using MyWebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;

namespace MyWebApi.Repositories.Implementations
{
    public class TeacherRepository(DataContext context) : ITeacherRepository
    {
        private readonly DataContext _context = context;
        public async Task<IEnumerable<Teacher>> GetAllAsync(string? keyword, string? sort, int page, int pageSize)
        {
            var query = _context.Teachers.AsQueryable();
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
            var query = _context.Teachers.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(t => t.Name.Contains(keyword) || t.Id.Contains(keyword));
            return await query.CountAsync();
        }
        public async Task<Teacher?> GetById(string id)
        {
            return await _context.Teachers.FindAsync(id);
        }
        public async Task AddAsync(Teacher teacher)
        {
            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Teacher teacher)
        {
            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
        }
    }
}
