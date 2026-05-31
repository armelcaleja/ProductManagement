using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<int?> RegisterUserAsync(UserRegistrationDto userCreationDto);
        Task<string?> LoginUserAsync(UserLoginDto userLoginDto);
    }
}
