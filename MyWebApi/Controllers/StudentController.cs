using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.DTOs;
using MyWebApi.Helpers;
using MyWebApi.Services.Implementations;
using MyWebApi.Services.Interfaces;
using ClosedXML.Excel;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController(IStudentService studentService, IFileService fileService, IUserAccessor userAccessor) : ControllerBase
    {
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
            var result = await studentService.GetAllStudentsAsync(keyword, sort, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Uploads an avatar image for a specific student.
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

            await studentService.UpdateAvatarUrlAsync(id, result.Url!);
            return Ok(new { url = result.Url });
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

            return CreatedAtAction(nameof(GetStudentById), new { id = createDto.Id }, createDto);
        }

        /// <summary>
        /// Creates a new student with Excel file.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportStudentsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn file Excel hợp lệ!");

            var studentsToInsert = new List<CreateStudentDto>();

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
                            var student = new CreateStudentDto
                            {
                                Id = row.Cell(1).GetValue<string>(),
                                Name = row.Cell(2).GetValue<string>(),
                                Birthday = row.Cell(3).GetValue<DateTime>(),
                                ClassId = row.Cell(4).GetValue<int>()
                            };
                            studentsToInsert.Add(student);
                        }
                    }
                }

                foreach (var std in studentsToInsert)
                {
                    await studentService.CreateStudentAsync(std);
                }

                return Ok(new { message = $"Đã import thành công {studentsToInsert.Count} sinh viên!" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi khi đọc file: {ex.Message}");
            }
        }

        /// <summary>
        /// Download example Excel file
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("template")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult DownloadTemplate()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("StudentTemplate");

                // Header
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Birthday (DD-MM-YYYY)";
                worksheet.Cell(1, 4).Value = "ClassId";

                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Dummy data
                worksheet.Cell(2, 1).Value = "SV001";
                worksheet.Cell(2, 2).Value = "Nguyễn Văn Mẫu";
                worksheet.Cell(2, 3).Value = "2004-01-01";
                worksheet.Cell(2, 4).Value = 1;

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Student_Import_Template.xlsx");
                }
            }
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

            return NoContent();
        }
    }
}