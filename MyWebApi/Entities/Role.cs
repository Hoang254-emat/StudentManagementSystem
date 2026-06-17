using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RoleName { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = [];
    }
}
