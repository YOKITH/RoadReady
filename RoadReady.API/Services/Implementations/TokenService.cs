using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

        public TokenService(
            IConfiguration configuration,
            IUserRepository userRepository,
            AppDbContext context)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

            if (user == null)
                return null;

            bool isPasswordValid =
                BCrypt.Net.BCrypt.Verify(
                    loginDto.Password,
                    user.PasswordHash);

            if (!isPasswordValid)
                return null;

            string accessToken = GenerateAccessToken(user);

            string refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.UserId,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };
        }

        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserId.ToString()),

                new Claim(
                    ClaimTypes.Email,
                    user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.Role),

                new Claim(
                    ClaimTypes.Name,
                    $"{user.FirstName} {user.LastName}")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];

            using var rng =
                RandomNumberGenerator.Create();

            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(
            RefreshTokenDto dto)
        {
            var storedToken =
                await _context.RefreshTokens
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(
                        r => r.Token == dto.RefreshToken);

            if (storedToken == null)
                return null;

            if (storedToken.ExpiryDate < DateTime.UtcNow)
                return null;

            var user = storedToken.User;

            string newAccessToken =
                GenerateAccessToken(user);

            string newRefreshToken =
                GenerateRefreshToken();

            storedToken.Token = newRefreshToken;
            storedToken.ExpiryDate =
                DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };
        }

        public async Task RevokeRefreshTokenAsync(
            string refreshToken)
        {
            var token =
                await _context.RefreshTokens
                    .FirstOrDefaultAsync(
                        x => x.Token == refreshToken);

            if (token != null)
            {
                _context.RefreshTokens.Remove(token);

                await _context.SaveChangesAsync();
            }
        }
    }
}