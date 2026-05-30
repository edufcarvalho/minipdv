using minipdv.Application.Interfaces;

namespace minipdv.Application.UseCases.Auth;

public class LogoutUseCase
{
    private readonly IAuthService _authService;

    public LogoutUseCase(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task ExecuteAsync(string token)
    {
        await _authService.LogoutAsync(token);
    }
}
