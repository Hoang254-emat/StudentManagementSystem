using ClosedXML.Excel;
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
            string cacheKey = CourseListCacheKey;

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
                return NotFound(new { message = "Course not found" });
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
        /// Creates a new course with Excel file (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("import")]
        public async Task<IActionResult> ImportCoursesFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("File không hợp lệ!");

            var coursesToInsert = new List<CreateCourseDto>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                    foreach (var row in rows)
                    {
                        coursesToInsert.Add(new CreateCourseDto
                        {
                            CourseName = row.Cell(1).GetValue<string>(),
                            SubjectId = row.Cell(2).GetValue<string>(),
                            TeacherId = row.Cell(3).GetValue<string>(),
                            ClassId = row.Cell(4).GetValue<int>()
                        });
                    }
                }
            }

            foreach (var dto in coursesToInsert)
            {
                await courseService.CreateCourseAsync(dto);
            }

            cache.Remove(CourseListCacheKey);
            return Ok(new { message = $"Import thành công {coursesToInsert.Count} khóa học!" });
        }

        /// <summary>
        /// Download Excel template (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("template")]
        public IActionResult DownloadTemplate()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("CourseTemplate");
                // Header
                worksheet.Cell(1, 1).Value = "CourseName";
                worksheet.Cell(1, 2).Value = "SubjectId";
                worksheet.Cell(1, 3).Value = "TeacherId";
                worksheet.Cell(1, 4).Value = "ClassId";

                worksheet.Range("A1:D1").Style.Font.Bold = true;
                worksheet.Range("A1:D1").Style.Fill.BackgroundColor = XLColor.Gold;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Course_Import_Template.xlsx");
                }
            }
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
                    return NotFound(new { message = "Course not found" });
                cache.Remove(CourseListCacheKey);
                return Ok(new { message = "Course updated successfully." });
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
                return NotFound(new { message = "Course not found" });
            cache.Remove(CourseListCacheKey);
            return Ok(new { message = "Course deleted successfully." });
        }

        /// <summary>
        /// Uploads curriculum file for a specific course (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/curriculum")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadCurriculum(int id, IFormFile file)
        {
            var result = await fileService.UploadFileAsync(file, "curriculum");

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            await courseService.UpdateCurriculumUrlAsync(id, result.Url!);

            return Ok(new { url = result.Url });
        }
    }
}
