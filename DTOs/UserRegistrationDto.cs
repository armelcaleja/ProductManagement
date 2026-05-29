using System.ComponentModel.DataAnnotations;

namespace ProductManagement.DTOs
{
    public class UserRegistrationDto
    {
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Username must be at least 8 characters.")]
        public string Username { get; set; } = String.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; } = String.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; } = String.Empty;


    }
}
