using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoadReady.API.Data;
using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Repositories.Interfaces;
using RoadReady.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RoadReady.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration,IUserRepository userRepository,
            AppDbContext context,ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user =await _userRepository.GetUserByEmailAsync(loginDto.Email);

                if (user == null)
                    throw new KeyNotFoundException("User not found.");

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password,user.PasswordHash);

                if (!isPasswordValid)
                    throw new UnauthorizedAccessException(
                        "Invalid email or password.");

                string accessToken = GenerateAccessToken(user);

                
                await _context.SaveChangesAsync();
                _logger.LogInformation("User logged in successfully: {Email}",user.Email);

                return new AuthResponseDto
                {
                    AccessToken = accessToken,
                    Email = user.Email,
                    Role = user.Role,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred during login.");

                throw;
            }
        }

        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(ClaimTypes.Role,user.Role),

                    new Claim(ClaimTypes.Name,$"{user.FirstName} {user.LastName}")
                };

            var key =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        _configuration["Jwt:Key"]!));

            var credentials =new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha256);

            var token =
                new JwtSecurityToken(
                    issuer:_configuration["Jwt:Issuer"],
                    audience:_configuration["Jwt:Audience"],
                    claims:claims,
                    expires:DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task LogoutAsync()
        {
            try
            {
                _logger.LogInformation("User logged out successfully.");

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError( ex,"Error occurred during logout.");

                throw;
            }
        }
    }
}