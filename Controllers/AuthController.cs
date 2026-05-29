using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto userRegistrationDto)
        {
            var newUserId = await _authService.RegisterUserAsync(userRegistrationDto);

            if (newUserId == null)
            {
                return BadRequest(new { message = "Username is already taken." });
            }

            var message = "User registered successfully!";

            var response = new UserRegistrationResponseDto
            { 
                UserId = newUserId,
                Username = userRegistrationDto.Username,
                Message = message
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            var token = await _authService.LoginUserAsync(userLoginDto);

            if (token == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            return Ok(new { Token = token });
        }
    }
}
