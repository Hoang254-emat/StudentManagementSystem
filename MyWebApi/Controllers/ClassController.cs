using Microsoft.AspNetCore.Authorization;
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
    public class ClassController(IClassService classService, IMemoryCache cache) : ControllerBase
    {
        private const string ClassListCacheKey = "ClassList_Cache";

        /// <summary>
        /// Retrieves a paginated list of classes with optional filtering and sorting.
        /// </summary>
        /// <param name="keyword">Search term to filter by classes name.</param>
        /// <param name="sort">Sorting criteria.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Items per page.</param>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<ClassDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] string? keyword, [FromQuery] string? sort, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            string cacheKey = $"ClassList_{keyword ?? "none"}_{sort ?? "none"}_{page}_{pageSize}";

            if (!cache.TryGetValue(cacheKey, out PagedResponse<ClassDto>? result))
            {
                result = await classService.GetAllClassesAsync(keyword, sort, page, pageSize);
                cache.Set(cacheKey, result, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(2) });
            }
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific class by its unique ID.
        /// </summary>
        /// <param name="id">The class ID.</param>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var @class = await classService.GetClassByIdAsync(id);
            if (@class == null)
                return NotFound("Class not found");
            return Ok(@class);
        }

        /// <summary>
        /// Creates a new class record (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateClassDto dto)
        {
            var createdClass = await classService.CreateClassAsync(dto);
            cache.Remove(ClassListCacheKey);
            return CreatedAtAction(nameof(GetById), new { id = createdClass?.Id }, createdClass);
        }

        /// <summary>
        /// Updates an existing class record (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] CreateClassDto dto)
        {
            var result = await classService.UpdateClassAsync(id, dto);
            if (!result)
                return NotFound("Class not found.");

            cache.Remove(ClassListCacheKey);
            return Ok("Class updated successfully.");
        }

        /// <summary>
        /// Deletes a class from the system (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await classService.DeleteClassAsync(id);
            if (!result)
                return NotFound("Class not found.");

            cache.Remove(ClassListCacheKey);
            return Ok("Class deleted successfully.");
        }
    }
}

