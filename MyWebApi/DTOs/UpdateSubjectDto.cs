using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs
{
    public class UpdateSubjectDto
    {
        [Required(ErrorMessage = "Subject code is required.")]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject name is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Subject name must be between 3 and 150 characters.")]
        public string SubjectName { get; set; } = string.Empty;

        [Range(1, 10, ErrorMessage = "Credits must be between 1 and 10.")]
        public int Credits { get; set; }

        public string? TeacherId { get; set; }
    }
}
