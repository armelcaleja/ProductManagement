using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.DTOs;
using ProductManagement.Interfaces;
using ProductManagement.Model;

namespace ProductManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<int?> RegisterUserAsync(UserRegistrationDto userCreationDto) 
        {
            var isUserExist = await _context.Users
                .AnyAsync(u => u.Username.ToLower() == userCreationDto.Username.ToLower());

            if (isUserExist)
            {
                return null;
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userCreationDto.Password);

            var newUser = new User
            {
                Username = userCreationDto.Username.Trim(),
                Password = hashedPassword
            };

            _context.Users.Add(newUser);

            await _context.SaveChangesAsync();

            return newUser.UserId;
        }

        public async Task<string?> LoginUserAsync(UserLoginDto userLoginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == userLoginDto.Username);

            if (user == null)
            {
                return null; 
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password);

            if (!isPasswordValid)
            {
                return null;
            }

            var userDto = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username
            };

            return _tokenService.GenerateToken(userDto);
        }  
    }
}
