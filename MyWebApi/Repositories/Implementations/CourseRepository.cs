using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Entities;
using MyWebApi.Repositories.Interfaces;

namespace MyWebApi.Repositories.Implementations
{
    public class CourseRepository(DataContext context) : ICourseRepository
    {
        private readonly DataContext _context = context;

        public async Task<IEnumerable<Course>> GetAllAsync(string? keyword, string? sort, int page, int pageSize)
        {
            var query = _context.Courses.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => s.CourseName.Contains(keyword) || s.Id.ToString().Contains(keyword));
            }
            if (!string.IsNullOrEmpty(sort))
            {
                if (sort.Equals("name", StringComparison.CurrentCultureIgnoreCase))
                {
                    query = query.OrderBy(s => s.CourseName);
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
            var query = _context.Courses.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(t => t.CourseName.Contains(keyword) || t.Id.ToString().Contains(keyword));
            return await query.CountAsync();
        }
        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Subject)
                .Include(c => c.Class)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task AddAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Course course)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }
}
