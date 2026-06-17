namespace MyWebApi.Entities
{
    public class Teacher
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }

        public ICollection<Course> Courses { get; set; } = [];
        public ICollection<Subject> Subjects { get; set; } = [];
    }
}
