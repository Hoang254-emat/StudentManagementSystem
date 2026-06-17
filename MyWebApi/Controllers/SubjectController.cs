using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyWebApi.DTOs;
using MyWebApi.Helpers;
using MyWebApi.Services.Interfaces;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController(ISubjectService subjectService, IMemoryCache cache) : ControllerBase
    {
        private const string SubjectListCacheKey = "SubjectList_Cache";

        /// <summary>
        /// Retrieves a paginated list of subjects.
        /// </summary>
        /// <param name="keyword">Filter by name or ID.</param>
        /// <param name="sort">Sort criteria.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Items per page.</param>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<SubjectDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSubjects([FromQuery] string? keyword, [FromQuery] string? sort, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            string cacheKey = $"SubjectList_{keyword ?? "none"}_{sort ?? "none"}_{page}_{pageSize}";

            if (!cache.TryGetValue(cacheKey, out PagedResponse<SubjectDto>? subjects))
            {
                subjects = await subjectService.GetAllSubjectsAsync(keyword, sort, page, pageSize);
                cache.Set(cacheKey, subjects, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(2) });
            }
            return Ok(subjects);
        }

        /// <summary>
        /// Creates a new subject (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await subjectService.CreateSubjectAsync(createDto);
            if (!result)
                return BadRequest("Failed to create subject.");
            cache.Remove(SubjectListCacheKey);
            return Ok("Subject created successfully.");
        }

        /// <summary>
        /// Updates an existing subject (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSubject(string id, [FromBody] SubjectDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await subjectService.UpdateSubjectAsync(id, updateDto);
            if (!result)
                return NotFound("Subject not found.");

            cache.Remove(SubjectListCacheKey);
            return Ok("Subject updated successfully.");
        }

        /// <summary>
        /// Deletes a subject (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSubject(string id)
        {
            var result = await subjectService.DeleteSubjectAsync(id);
            if (!result)
                return NotFound("Subject not found.");
            cache.Remove(SubjectListCacheKey);
            return Ok("Subject deleted successfully.");
        }
    }
}
