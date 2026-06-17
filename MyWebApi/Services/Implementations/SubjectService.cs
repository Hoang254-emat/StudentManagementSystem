using AutoMapper;
using MyWebApi.DTOs;
using MyWebApi.Entities;
using MyWebApi.Helpers;
using MyWebApi.Repositories.Interfaces;
using MyWebApi.Services.Interfaces;

namespace MyWebApi.Services.Implementations
{
    public class SubjectService(ISubjectRepository subjectRepo, IMapper mapper) : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepo = subjectRepo;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResponse<SubjectDto>> GetAllSubjectsAsync(string? keyword, string? sort, int page, int pageSize)
        {
            var subjects = await _subjectRepo.GetAllAsync(keyword, sort, page, pageSize); 
            var totalItems = await _subjectRepo.GetTotalCountAsync(keyword);

            return new PagedResponse<SubjectDto>
            {
                Data = _mapper.Map<IEnumerable<SubjectDto>>(subjects),
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }

        public async Task<bool> CreateSubjectAsync(CreateSubjectDto createDto)
        {
            if (await _subjectRepo.ExistsAsync(createDto.Id))
                return false;
            var subject = _mapper.Map<Subject>(createDto);
            await _subjectRepo.AddAsync(subject);
            return true;
        }
        public async Task<bool> UpdateSubjectAsync(string id, SubjectDto updateDto)
        {
            var existingSubject = await _subjectRepo.GetByIdAsync(id);
            if (existingSubject == null) return false;
            _mapper.Map(updateDto, existingSubject);
            await _subjectRepo.UpdateAsync(existingSubject);
            return true;
        }
        public async Task<bool> DeleteSubjectAsync(string id)
        {
            var existingSubject = await _subjectRepo.GetByIdAsync(id);
            if (existingSubject == null) return false;
            await _subjectRepo.DeleteAsync(existingSubject);
            return true;
        }
    }
}

