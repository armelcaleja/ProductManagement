using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
