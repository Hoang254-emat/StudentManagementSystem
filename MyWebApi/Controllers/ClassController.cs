using ClosedXML.Excel;
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
            string cacheKey = ClassListCacheKey;

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
        /// Creates a new class with Excel file (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportClassesFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn file Excel hợp lệ!");

            var classesToInsert = new List<CreateClassDto>();

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
                            classesToInsert.Add(new CreateClassDto
                            {
                                ClassName = row.Cell(1).GetValue<string>(),
                                TeacherId = row.Cell(2).GetValue<string>()
                            });
                        }
                    }
                }

                foreach (var cls in classesToInsert)
                {
                    await classService.CreateClassAsync(cls);
                }

                return Ok(new { message = $"Đã import thành công {classesToInsert.Count} lớp học!" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi khi đọc file: {ex.Message}");
            }
        }

        /// <summary>
        /// Download Template Excel (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("template")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult DownloadTemplate()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ClassTemplate");

                // Header
                worksheet.Cell(1, 1).Value = "ClassName";
                worksheet.Cell(1, 2).Value = "TeacherId";

                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Dữ liệu mẫu
                worksheet.Cell(2, 1).Value = "Kiếm Tu 1";
                worksheet.Cell(2, 2).Value = "GV0001";

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Class_Import_Template.xlsx");
                }
            }
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
                return NotFound(new { message = "Class not found." });

            cache.Remove(ClassListCacheKey);
            return Ok(new { message = "Class updated successfully." });
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
                return NotFound(new { message = "Class not found." });

            cache.Remove(ClassListCacheKey);
            return Ok(new { message = "Class deleted successfully." });
        }
    }
}

