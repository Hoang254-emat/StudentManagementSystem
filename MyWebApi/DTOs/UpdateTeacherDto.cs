using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs
{
    public class UpdateTeacherDto
    {
        [Required(ErrorMessage = "Teacher name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Teacher name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Teacher email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; } = string.Empty;
    }
}
