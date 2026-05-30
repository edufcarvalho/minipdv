using FluentValidation;
using minipdv.Application.DTOs.Auth;
using minipdv.Application.Interfaces;

namespace minipdv.Application.UseCases.Auth;

public class LoginUseCase
{
    private readonly IAuthService _authService;
    private readonly IValidator<LoginRequest> _validator;

    public LoginUseCase(IAuthService authService, IValidator<LoginRequest> validator)
    {
        _authService = authService;
        _validator = validator;
    }

    public async Task<AuthResponse> ExecuteAsync(LoginRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);
        return await _authService.LoginAsync(request);
    }
}
