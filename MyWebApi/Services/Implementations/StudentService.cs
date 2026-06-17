using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyWebApi.DTOs;
using MyWebApi.Entities;
using MyWebApi.Helpers;
using MyWebApi.Repositories.Interfaces;
using MyWebApi.Services.Interfaces;

namespace MyWebApi.Services.Implementations
{
    public class StudentService(IStudentRepository studentRepository, IMapper mapper) : IStudentService
    {
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResponse<StudentDto>> GetAllStudentsAsync(string? keyword, string? sort, int page, int pageSize)
        {
            var students = await _studentRepository.GetAllStudents(keyword, sort, page, pageSize);
            var totalItems = await _studentRepository.GetTotalCountAsync(keyword);

            return new PagedResponse<StudentDto>
            {
                Data = _mapper.Map<IEnumerable<StudentDto>>(students),
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }

        public async Task<StudentDto?> GetStudentByIdAsync(string id)
        {
            var student = await _studentRepository.GetStudentById(id);
            if (student == null) return null;
            return _mapper.Map<StudentDto>(student);
        }

        public async Task<bool> CreateStudentAsync(CreateStudentDto createDto)
        {
            if (await _studentRepository.StudentExists(createDto.Id))
            {
                return false; 
            }
            var student = _mapper.Map<Student>(createDto);
            await _studentRepository.AddAsync(student);
            return true;
        }

        public async Task<bool> UpdateStudentAsync(string id, UpdateStudentDto updateDto)
        {
            var existingStudent = await _studentRepository.GetStudentById(id);
            if (existingStudent == null)
            {
                return false; 
            }
            _mapper.Map(updateDto, existingStudent);
            await _studentRepository.UpdateAsync(existingStudent);
            return true;
        }

        public async Task<bool> DeleteStudentAsync(string id)
        {
            var existingStudent = await _studentRepository.GetStudentById(id);
            if (existingStudent == null)
            {
                return false; 
            }
            await _studentRepository.DeleteAsync(existingStudent);
            return true;
        }

        public async Task<bool> UpdateAvatarUrlAsync(string id, string avatarUrl)
        {
            var student = await _studentRepository.GetStudentById(id);
            if (student == null) return false;

            student.AvatarUrl = avatarUrl;

            await _studentRepository.UpdateAsync(student);
            return true;
        }
    }
}
