using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Entities;
using MyWebApi.Repositories.Interfaces;

namespace MyWebApi.Repositories.Implementations
{
    public class ClassRepository(DataContext context) : IClassRepository
    {
        private readonly DataContext _context = context;

        public async Task<IEnumerable<Class>> GetAllAsync(string? keyword, string? sort, int page, int pageSize)
        {
            var query = _context.Classes.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => s.ClassName.Contains(keyword) || s.Id.ToString().Contains(keyword));
            }
            if (!string.IsNullOrEmpty(sort))
            {
                if (sort.Equals("name", StringComparison.CurrentCultureIgnoreCase))
                {
                    query = query.OrderBy(s => s.ClassName);
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
            var query = _context.Classes.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(t => t.ClassName.Contains(keyword) || t.Id.ToString().Contains(keyword));
            return await query.CountAsync();
        }
        public async Task<Class?> GetByIdAsync(int id) => await _context.Classes.FindAsync(id);
        public async Task AddAsync(Class @class) 
        { 
            await _context.Classes.AddAsync(@class); 
            await _context.SaveChangesAsync(); 
        }
        public async Task UpdateAsync(Class @class) 
        { 
            _context.Classes.Update(@class); 
            await _context.SaveChangesAsync(); 
        }
        public async Task DeleteAsync(Class @class) 
        { 
            _context.Classes.Remove(@class); 
            await _context.SaveChangesAsync(); 
        }
    }
}
