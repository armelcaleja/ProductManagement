using System.ComponentModel.DataAnnotations;

namespace ProductManagement.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; } = String.Empty;
    }
}
