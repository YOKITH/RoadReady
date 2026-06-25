using RoadReady.API.DTOs;

namespace RoadReady.API.Services.Interfaces
{
    public interface ITokenService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);

        string GenerateAccessToken(Models.User user);

        Task LogoutAsync();

    }
}