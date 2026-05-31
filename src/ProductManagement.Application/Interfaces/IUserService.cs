using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Interfaces
{
    public interface IUserService
    {
        IEnumerable<UserDto> GetAllUsers();
        void CreateUser();
    }
}
