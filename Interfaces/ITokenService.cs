using ProductManagement.DTOs;
using ProductManagement.Model;

namespace ProductManagement.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken(UserDto userDto);
    }
}
