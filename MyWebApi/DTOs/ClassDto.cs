namespace MyWebApi.DTOs
{
    public class ClassDto
    {
        public int Id { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string? TeacherId { get; set; }
        public string? TeacherName { get; set; }
    }
}
