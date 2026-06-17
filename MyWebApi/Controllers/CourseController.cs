using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyWebApi.DTOs;
using MyWebApi.Helpers;
using MyWebApi.Services.Implementations;
using MyWebApi.Services.Interfaces;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController(ICourseService courseService, IFileService fileService, IMemoryCache cache) : ControllerBase
    {
        private const string CourseListCacheKey = "CourseList_Cache";

        /// <summary>
        /// Retrieves a paginated list of courses with optional filtering and sorting.
        /// </summary>
        /// <param name="keyword">Search term to filter by course name.</param>
        /// <param name="sort">Sorting criteria.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Items per page.</param>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<CourseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] string? keyword, [FromQuery] string? sort, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            string cacheKey = $"CourseList_{keyword ?? "none"}_{sort ?? "none"}_{page}_{pageSize}";

            if (!cache.TryGetValue(cacheKey, out PagedResponse<CourseDto>? result))
            {
                result = await courseService.GetAllCoursesAsync(keyword, sort, page, pageSize);
                cache.Set(cacheKey, result, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(2) });
            }
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific course by its unique ID.
        /// </summary>
        /// <param name="id">The course ID.</param>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound("Course not found");
            return Ok(course);
        }

        /// <summary>
        /// Creates a new course record (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
        {
            var createdCourse = await courseService.CreateCourseAsync(dto);
            cache.Remove(CourseListCacheKey);
            return CreatedAtAction(nameof(GetById), new { id = createdCourse.Id }, createdCourse);
        }

        /// <summary>
        /// Updates an existing course record (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCourseDto dto)
        {
            try
            {
                var result = await courseService.UpdateCourseAsync(id, dto);
                if (!result)
                    return NotFound("Course not found.");
                cache.Remove(CourseListCacheKey);
                return Ok("Course updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a course from the system (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await courseService.DeleteCourseAsync(id);
            if (!result)
                return NotFound("Course not found.");
            cache.Remove(CourseListCacheKey);
            return Ok("Course deleted successfully.");
        }

        /// <summary>
        /// Uploads curriculum file for a specific course (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/curriculum")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadCurriculum(int id, IFormFile file)
        {
            var url = await fileService.UploadFileAsync(file, "curriculum");

            await courseService.UpdateCurriculumUrlAsync(id, url);

            return Ok(new { url });
        }
    }
}
