using AutoMapper;
using MyWebApi.DTOs;
using MyWebApi.Entities;
using MyWebApi.Helpers;
using MyWebApi.Repositories.Implementations;
using MyWebApi.Repositories.Interfaces;
using MyWebApi.Services.Interfaces;

namespace MyWebApi.Services.Implementations
{
    public class TeacherService(ITeacherRepository teacherRepo, IMapper mapper) : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepo = teacherRepo;
        private readonly IMapper _mapper = mapper;

        public async Task<bool> CreateTeacherAsync(CreateTeacherDto createDto)
        {
            var teacher = _mapper.Map<Teacher>(createDto);
            await _teacherRepo.AddAsync(teacher);
            return true;
        }

        public async Task<PagedResponse<TeacherDto>> GetAllTeachersAsync(string? keyword, string? sort, int page, int pageSize)
        {
            var teachers = await _teacherRepo.GetAllAsync(keyword, sort, page, pageSize);
            var totalItems = await _teacherRepo.GetTotalCountAsync(keyword);

            return new PagedResponse<TeacherDto>
            {
                Data = _mapper.Map<IEnumerable<TeacherDto>>(teachers),
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }

        public async Task<TeacherDto?> GetByIdAsync(string id)
        {
            var teacher = await _teacherRepo.GetById(id);
            if (teacher == null) return null;
            return _mapper.Map<TeacherDto>(teacher);
        }

        public async Task<bool> UpdateTeacherAsync(string id, TeacherDto updateDto)
        {
            var existingTeacher = await _teacherRepo.GetById(id);
            if (existingTeacher == null) return false;
            _mapper.Map(updateDto, existingTeacher);
            await _teacherRepo.UpdateAsync(existingTeacher);
            return true;
        }
        public async Task<bool> DeleteTeacherAsync(string id)
        {
            var existingTeacher = await _teacherRepo.GetById(id);
            if (existingTeacher == null) return false;
            await _teacherRepo.DeleteAsync(existingTeacher);
            return true;
        }
        public async Task<bool> UpdateAvatarUrlAsync(string id, string avatarUrl)
        {
            var teacher = await _teacherRepo.GetById(id);
            if (teacher == null) return false;

            teacher.AvatarUrl = avatarUrl;

            await _teacherRepo.UpdateAsync(teacher);
            return true;
        }
    }
}
