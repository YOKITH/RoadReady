using RoadReady.API.DTOs;

namespace RoadReady.API.Services.Interfaces
{
    public interface ITokenService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);

        string GenerateAccessToken(Models.User user);

        string GenerateRefreshToken();

        Task<AuthResponseDto?> RefreshTokenAsync(
            RefreshTokenDto refreshTokenDto);

        Task RevokeRefreshTokenAsync(string refreshToken);


    }
}