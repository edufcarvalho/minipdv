using FluentValidation;
using minipdv.Application.DTOs.Auth;
using minipdv.Application.Interfaces;

namespace minipdv.Application.UseCases.Auth;

public class RegisterUseCase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterRequest> _validator;

    public RegisterUseCase(IAuthService authService, IValidator<RegisterRequest> validator)
    {
        _authService = authService;
        _validator = validator;
    }

    public async Task<AuthResponse> ExecuteAsync(RegisterRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);
        return await _authService.RegisterAsync(request);
    }
}
