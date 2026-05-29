using Microsoft.AspNetCore.Mvc;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;
using ProductManagement.Services;

namespace ProductManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IEnumerable<UserDto> GetAllUsers()
        {
            var products = _userService.GetAllUsers();

            return products;
        }
    }
}
