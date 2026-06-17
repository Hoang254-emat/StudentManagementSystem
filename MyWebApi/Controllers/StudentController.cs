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
    public class StudentController(IStudentService studentService, IFileService fileService, IMemoryCache cache, IUserAccessor userAccessor) : ControllerBase
    {
        private const string StudentListCacheKey = "StudentList_Cache";

        /// <summary>
        /// Retrieves a paginated list of students.
        /// </summary>
        /// <param name="keyword">Search term to filter by student name or ID.</param>
        /// <param name="sort">Sorting criteria (e.g., "name", "id").</param>
        /// <param name="page">The page number to retrieve (default is 1).</param>
        /// <param name="pageSize">Number of items per page (default is 10).</param>
        /// <returns>A PagedResponse containing student data and pagination info.</returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<StudentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllStudents([FromQuery] string? keyword, [FromQuery] string? sort, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            string cacheKey = $"StudentList_{keyword ?? "none"}_{sort ?? "none"}_{page}_{pageSize}";

            if (!cache.TryGetValue(cacheKey, out PagedResponse<StudentDto>? result))
            {
                result = await studentService.GetAllStudentsAsync(keyword, sort, page, pageSize);
                cache.Set(cacheKey, result, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(2) });
            }
            return Ok(result);
        }

        /// <summary>
        /// Uploads an avatar image for a specific student.
        /// </summary>
        /// <param name="id">The unique ID of the student.</param>
        /// <param name="file">The image file to upload.</param>
        /// <returns>The URL of the uploaded avatar.</returns>
        [Authorize]
        [HttpPost("{id}/avatar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadAvatar(string id, IFormFile file)
        {
            var url = await fileService.UploadFileAsync(file, "avatars");
            await studentService.UpdateAvatarUrlAsync(id, url);
            return Ok(new { url });
        }

        /// <summary>
        /// Retrieves a student's information by their ID.
        /// </summary>
        /// <param name="id">The unique ID of the student.</param>
        /// <returns>Student profile data.</returns>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentById(string id)
        {
            var student = await studentService.GetStudentByIdAsync(id);
            if (student == null) return NotFound();
            return Ok(student);
        }

        /// <summary>
        /// Creates a new student record (Admin only).
        /// </summary>
        /// <param name="createDto">Student registration details.</param>
        /// <response code="201">Student successfully created.</response>
        /// <response code="409">Student ID already exists.</response>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto createDto)
        {
            var success = await studentService.CreateStudentAsync(createDto);
            if (!success) return Conflict("A student with the same ID already exists.");
            cache.Remove(StudentListCacheKey);
            return CreatedAtAction(nameof(GetStudentById), new { id = createDto.Id }, createDto);
        }

        /// <summary>
        /// Retrieves the profile of the currently logged-in student.
        /// </summary>
        [Authorize]
        [HttpGet("my-profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile()
        {
            var currentUserId = userAccessor.GetCurrentUserId();
            Console.WriteLine("DEBUG ID: " + currentUserId);
            var profile = await studentService.GetStudentByIdAsync(currentUserId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        /// <summary>
        /// Updates an existing student record.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateStudent(string id, [FromBody] UpdateStudentDto updateDto)
        {
            var CurrentUserId = userAccessor.GetCurrentUserId();
            var CurrentUserRole = userAccessor.GetCurrentRole();

            if (CurrentUserRole != "Admin" && id != CurrentUserId)
                return Forbid();

            var success = await studentService.UpdateStudentAsync(id, updateDto);
            if (!success) return NotFound();
            cache.Remove(StudentListCacheKey);
            return NoContent();
        }

        /// <summary>
        /// Deletes an existing student record.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var CurrentUserId = userAccessor.GetCurrentUserId();
            var CurrentUserRole = userAccessor.GetCurrentRole();

            if (CurrentUserRole != "Admin" && id != CurrentUserId)
                return Forbid();

            var success = await studentService.DeleteStudentAsync(id);
            if (!success) return NotFound();
            cache.Remove(StudentListCacheKey);
            return NoContent();
        }
    }
}
