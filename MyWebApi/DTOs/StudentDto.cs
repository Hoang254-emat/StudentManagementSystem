namespace MyWebApi.DTOs
{
    public class StudentDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTime Birthday { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = null!;
        public string AvatarUrl { get; set; }
    }
}
