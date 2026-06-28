using System;
using System.Collections.Generic;

namespace MyWebApi.Entities
{
    public class Student
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string? AvatarUrl { get; set; }

        public int ClassId { get; set; }
        public Class? Class { get; set; }
    }
}
