using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities
{
    public class Class
    {
        [Key]
        public int Id { get; set; }
        public string ClassName { get; set; } = string.Empty;

        public ICollection<Course> Courses { get; set; } = [];

        public List<Student> Students { get; set; } = [];
    }
}
