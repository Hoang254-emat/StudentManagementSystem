using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Entities;
using MyWebApi.Repositories.Interfaces;

namespace MyWebApi.Repositories.Implementations
{
    public class SubjectRepository(DataContext context) : ISubjectRepository
    {
        private readonly DataContext _context = context;

        public async Task<IEnumerable<Subject>> GetAllAsync(string? keyword, string? sort, int page, int pageSize)
        {
            var query = _context.Subjects.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => s.SubjectName.Contains(keyword) || s.SubjectId.ToString().Contains(keyword));
            }
            if (!string.IsNullOrEmpty(sort))
            {
                if (sort.Equals("name", StringComparison.CurrentCultureIgnoreCase))
                {
                    query = query.OrderBy(s => s.SubjectName);
                }
                else if (sort.Equals("id", StringComparison.CurrentCultureIgnoreCase))
                {
                    query = query.OrderBy(s => s.SubjectId);
                }
            }
            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? keyword)
        {
            var query = _context.Subjects.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(t => t.SubjectName.Contains(keyword) || t.SubjectId.Contains(keyword));
            return await query.CountAsync();
        }
        public async Task AddAsync(Subject subject)
        {
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Subjects.AnyAsync(s => s.SubjectId == id);
        }
        public async Task<Subject?> GetByIdAsync(string id)
        {
            return await _context.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.SubjectId == id);
        }
        public async Task UpdateAsync(Subject subject)
        {
            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Subject subject)
        {
            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
        }
    }
}
