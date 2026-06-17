using System.Collections.Generic;

namespace MyWebApi.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? CurriculumUrl { get; set; }

        public string SubjectId { get; set; } = string.Empty;
        public Subject Subject { get; set; } = null!;

        public string TeacherId { get; set; } = string.Empty;
        public Teacher Teacher { get; set; } = null!;

        public int ClassId { get; set; }
        public Class Class { get; set; } = null!;
    }
}
