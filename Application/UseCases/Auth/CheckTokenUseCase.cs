using minipdv.Application.DTOs.Auth;
using minipdv.Application.Interfaces;

namespace minipdv.Application.UseCases.Auth;

public class CheckTokenUseCase
{
    private readonly IAuthService _authService;

    public CheckTokenUseCase(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<CheckTokenResponse> ExecuteAsync(string token)
    {
        return await _authService.CheckTokenAsync(token);
    }
}
