using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(UserDto userDto);
    }
}
