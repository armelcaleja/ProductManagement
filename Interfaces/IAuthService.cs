using Microsoft.AspNetCore.Mvc;
using ProductManagement.DTOs;

namespace ProductManagement.Interfaces
{
    public interface IAuthService
    {
        public Task<int?> RegisterUserAsync(UserRegistrationDto userCreationDto);
        public Task<string?> LoginUserAsync(UserLoginDto userLoginDto);
    }
}
