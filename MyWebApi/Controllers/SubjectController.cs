using ClosedXML.Excel;
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
            string cacheKey = SubjectListCacheKey;

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
            return Ok(new { message = "Subject created successfully." });
        }

        /// <summary>
        /// Import excel file to create Subject (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportSubjectsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn file Excel hợp lệ!");

            var subjectsToInsert = new List<CreateSubjectDto>();

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
                            subjectsToInsert.Add(new CreateSubjectDto
                            {
                                Id = row.Cell(1).GetValue<string>(),
                                SubjectName = row.Cell(2).GetValue<string>(),
                                Credits = row.Cell(3).GetValue<int>(),
                                TeacherId = row.Cell(4).GetValue<string>() 
                            });
                        }
                    }
                }

                foreach (var sub in subjectsToInsert)
                {
                    await subjectService.CreateSubjectAsync(sub);
                }

                return Ok(new { message = $"Đã import thành công {subjectsToInsert.Count} môn học!" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi khi đọc file: {ex.Message}");
            }
        }

        /// <summary>
        /// Download template excel file (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("template")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult DownloadTemplate()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("SubjectTemplate");

                // Header
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "SubjectName";
                worksheet.Cell(1, 3).Value = "Credits";
                worksheet.Cell(1, 4).Value = "TeacherId";

                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Dữ liệu mẫu
                worksheet.Cell(2, 1).Value = "KP0001";
                worksheet.Cell(2, 2).Value = "Kiếm Pháp Nhập Môn";
                worksheet.Cell(2, 3).Value = 3;
                worksheet.Cell(2, 4).Value = "GV0001";

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Subject_Import_Template.xlsx");
                }
            }
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
                return NotFound(new { message = "Subject not found" });

            cache.Remove(SubjectListCacheKey);
            return Ok(new { message = "Subject updated successfully." });
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
                return NotFound(new { message = "Subject not found" });
            cache.Remove(SubjectListCacheKey);
            return Ok(new { message = "Subject deleted successfully." });
        }
    }
}
