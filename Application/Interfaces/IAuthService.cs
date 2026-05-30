using minipdv.Application.DTOs.Auth;

namespace minipdv.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task LogoutAsync(string jwtId);
    Task<CheckTokenResponse> CheckTokenAsync(string token);
}
