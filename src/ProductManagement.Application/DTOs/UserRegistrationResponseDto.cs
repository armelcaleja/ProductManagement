namespace ProductManagement.Application.DTOs
{
    public class UserRegistrationResponseDto
    {
        public int? UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
