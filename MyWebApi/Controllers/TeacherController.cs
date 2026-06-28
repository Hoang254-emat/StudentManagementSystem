using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyWebApi.DTOs;
using MyWebApi.Helpers;
using MyWebApi.Services.Implementations;
using MyWebApi.Services.Interfaces;
using ClosedXML.Excel;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController(ITeacherService teacherService,IFileService fileService, IMemoryCache cache, IUserAccessor userAccessor) : ControllerBase
    {
        private const string TeacherListCacheKey = "TeacherList_Cache";

        /// <summary>
        /// Retrieves a paginated list of teachers with optional filtering and sorting.
        /// </summary>
        /// <param name="keyword">Search term to filter by teacher name or ID.</param>
        /// <param name="sort">Sorting criteria (e.g., "name", "id").</param>
        /// <param name="page">The page number to retrieve (default is 1).</param>
        /// <param name="pageSize">Number of items per page (default is 10).</param>
        /// <returns>A paginated response containing teacher data.</returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<TeacherDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTeachers([FromQuery] string? keyword, [FromQuery] string? sort, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            string cacheKey = $"TeacherList_{keyword ?? "none"}_{sort ?? "none"}_{page}_{pageSize}";

            if (!cache.TryGetValue(cacheKey, out PagedResponse<TeacherDto>? result))
            {
                result = await teacherService.GetAllTeachersAsync(keyword, sort, page, pageSize);
                cache.Set(cacheKey, result, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(2) });
            }
            return Ok(result);
        }

        /// <summary>
        /// Creates a new teacher record (Admin only).
        /// </summary>
        /// <param name="createDto">Teacher registration details.</param>
        /// <response code="200">Teacher created successfully.</response>
        /// <response code="400">Invalid data provided.</response>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await teacherService.CreateTeacherAsync(createDto);
            if (!result)
                return BadRequest("Failed to create teacher.");
            cache.Remove(TeacherListCacheKey);
            return Ok(new { message = "Teacher created successfully." });
        }

        /// <summary>
        /// Creates new teachers using an Excel file.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportTeachersFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn file Excel hợp lệ!");

            var teachersToInsert = new List<CreateTeacherDto>();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                        foreach (var row in rows)
                        {
                            var teacher = new CreateTeacherDto
                            {
                                Id = row.Cell(1).GetValue<string>(),
                                Name = row.Cell(2).GetValue<string>(),
                                Email = row.Cell(3).GetValue<string>() 
                            };
                            teachersToInsert.Add(teacher);
                        }
                    }
                }

                foreach (var tch in teachersToInsert)
                {
                    await teacherService.CreateTeacherAsync(tch);
                }

                cache.Remove(TeacherListCacheKey);

                return Ok(new { message = $"Đã import thành công {teachersToInsert.Count} giáo viên!" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi khi đọc file: {ex.Message}");
            }
        }

        /// <summary>
        /// Download example Excel file for Teachers
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("template")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult DownloadTemplate()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("TeacherTemplate");

                // Header
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Email";

                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Dummy data
                worksheet.Cell(2, 1).Value = "GV0001";
                worksheet.Cell(2, 2).Value = "Kiếm Thần";
                worksheet.Cell(2, 3).Value = "kiemthan@tongmon.com";

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Teacher_Import_Template.xlsx");
                }
            }
        }

        /// <summary>
        /// Retrieves the profile of the currently logged-in teacher.
        /// </summary>
        [HttpGet("my-profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile()
        {
            var currentUserId = userAccessor.GetCurrentUserId();
            Console.WriteLine("DEBUG ID: " + currentUserId);
            var profile = await teacherService.GetByIdAsync(currentUserId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        /// <summary>
        /// Uploads an avatar image for a specific teacher.
        /// </summary>
        [Authorize]
        [HttpPost("{id}/avatar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadAvatar(string id, IFormFile file)
        {
            var result = await fileService.UploadFileAsync(file, "avatars");

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            await teacherService.UpdateAvatarUrlAsync(id, result.Url!);
            return Ok(new { url = result.Url });
        }

        /// <summary>
        /// Updates an existing teacher's information.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTeacher(string id, [FromBody] TeacherDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await teacherService.UpdateTeacherAsync(id, updateDto);
            if (!result)
                return NotFound(new { message = "Teacher not found" });
            cache.Remove(TeacherListCacheKey);
            return Ok(new { message = "Teacher updated successfully." });
        }

        /// <summary>
        /// Deletes a teacher record from the system.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTeacher(string id)
        {
            var result = await teacherService.DeleteTeacherAsync(id);
            if (!result)
                return NotFound(new { message = "Teacher not found" });
            cache.Remove(TeacherListCacheKey);
            return Ok(new { message = "Teacher deleted successfully." });
        }
    }
}
