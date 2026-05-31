using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Application.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;
    }
}
