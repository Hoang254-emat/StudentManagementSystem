using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs
{
    public class CreateCourseDto
    {
        [Required(ErrorMessage = "Course name is required.")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Coursename must between 10 and 100 characters.")]
        public string CourseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject ID is required.")]
        public string SubjectId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Teacher ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid teacher ID.")]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Class ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid class ID.")]
        public int ClassId { get; set; }
    }
}
