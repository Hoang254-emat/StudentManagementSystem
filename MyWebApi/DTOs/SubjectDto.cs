namespace MyWebApi.DTOs
{
    public class SubjectDto
    {
        public string Id { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string? TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
    }
}
