namespace ProductManagement.DTOs
{
    public class UserRegistrationResponseDto
    {
        public int? UserId { get; set; }
        public string Username { get; set; } = String.Empty;
        public string Message { get; set; } = String.Empty;
    }
}
