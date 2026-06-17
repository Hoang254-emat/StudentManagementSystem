using AutoMapper;
using MyWebApi.DTOs;
using MyWebApi.Entities;
using MyWebApi.Helpers;
using MyWebApi.Repositories.Interfaces;
using MyWebApi.Services.Interfaces;

namespace MyWebApi.Services.Implementations
{
    public class ClassService(IClassRepository repo, IMapper mapper) : IClassService
    {
        private readonly IClassRepository _repo = repo;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResponse<ClassDto>> GetAllClassesAsync(string? keyword, string? sort, int page, int pageSize)
        {
            var classes = await _repo.GetAllAsync(keyword, sort, page, pageSize);
            var totalItems = await _repo.GetTotalCountAsync(keyword);

            return new PagedResponse<ClassDto>
            {
                Data = _mapper.Map<IEnumerable<ClassDto>>(classes),
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }
        public async Task<ClassDto?> GetClassByIdAsync(int id) => _mapper.Map<ClassDto>(await _repo.GetByIdAsync(id));
        public async Task<ClassDto?> CreateClassAsync(CreateClassDto dto) 
        { 
            var entity = _mapper.Map<Class>(dto); 
            await _repo.AddAsync(entity); 
            return _mapper.Map<ClassDto>(entity); 
        }
        public async Task<bool> UpdateClassAsync(int id, CreateClassDto dto) 
        { var entity = await _repo.GetByIdAsync(id); 
            if (entity == null) 
                return false; 
            _mapper.Map(dto, entity); 
            await _repo.UpdateAsync(entity); 
            return true; 
        }
        public async Task<bool> DeleteClassAsync(int id) 
        { 
            var entity = await _repo.GetByIdAsync(id); 
            if (entity == null) 
                return false; 
            await _repo.DeleteAsync(entity); 
            return true; 
        }
    }
}
