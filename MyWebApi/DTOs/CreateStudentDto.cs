using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs
{
    public class CreateStudentDto
    {
        [Required(ErrorMessage = "Student Id is required.")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Student ID must be between 4 and 15 characters.")]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Student name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Student name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date of birth format.")]
        public DateTime Birthday { get; set; }

        [Required(ErrorMessage = "Class Id is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid class ID.")]
        public int ClassId { get; set; }
    }
}
