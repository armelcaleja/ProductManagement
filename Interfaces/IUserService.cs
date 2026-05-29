using ProductManagement.DTOs;

namespace ProductManagement.Interfaces
{
    public interface IUserService
    {
        public IEnumerable<UserDto> GetAllUsers();
        public void CreateUser();
    }
}
