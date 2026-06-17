using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs
{
    public class CreateClassDto
    {
        [Required(ErrorMessage = "Class name is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Class name must be between 6 and 100 characters.")]
        public string ClassName { get; set; } = string.Empty;
    }
}
