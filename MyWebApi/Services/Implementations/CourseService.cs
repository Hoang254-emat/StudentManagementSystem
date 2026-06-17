using AutoMapper;
using MyWebApi.DTOs;
using MyWebApi.Entities;
using MyWebApi.Helpers;
using MyWebApi.Repositories.Interfaces;
using MyWebApi.Services.Interfaces;

namespace MyWebApi.Services.Implementations
{
    public class CourseService(ICourseRepository repo, IMapper mapper) : ICourseService
    {
        private readonly ICourseRepository _repo = repo;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResponse<CourseDto>> GetAllCoursesAsync(string? keyword, string? sort, int page, int pageSize)
        {
            var courses = await _repo.GetAllAsync(keyword, sort, page, pageSize); 
            var totalItems = await _repo.GetTotalCountAsync(keyword);

            return new PagedResponse<CourseDto>
            {
                Data = _mapper.Map<IEnumerable<CourseDto>>(courses),
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }
        public async Task<CourseDto?> GetCourseByIdAsync(int id) => _mapper.Map<CourseDto>(await _repo.GetByIdAsync(id));
        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto) 
        { 
            var entity = _mapper.Map<Course>(dto); 
            await _repo.AddAsync(entity); 
            var fullCourseData = await _repo.GetByIdAsync(entity.Id); 
            return _mapper.Map<CourseDto>(fullCourseData); 
        }
        public async Task<bool> UpdateCourseAsync(int id, CreateCourseDto dto) 
        { 
            var entity = await _repo.GetByIdAsync(id); 
            if (entity == null) return false; _mapper.Map(dto, entity); 
            await _repo.UpdateAsync(entity); 
            return true; 
        }
        public async Task<bool> DeleteCourseAsync(int id) 
        { 
            var entity = await _repo.GetByIdAsync(id); 
            if (entity == null) return false; 
            await _repo.DeleteAsync(entity); 
            return true; 
        }
        public async Task<bool> UpdateCurriculumUrlAsync(int id, string curriculumUrl)
        {
            var course = await _repo.GetByIdAsync(id);
            if (course == null) return false;

            course.CurriculumUrl = curriculumUrl;

            await _repo.UpdateAsync(course);
            return true;
        }
    }
}
