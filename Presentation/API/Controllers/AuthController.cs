using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minipdv.Application.DTOs.Auth;
using minipdv.Application.Interfaces;

namespace minipdv.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IValidator<RegisterRequest> _registerValidator;

    public AuthController(
        IAuthService authService,
        IValidator<LoginRequest> loginValidator,
        IValidator<RegisterRequest> registerValidator)
    {
        _authService = authService;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            await _loginValidator.ValidateAndThrowAsync(request);

            var result = await _authService.LoginAsync(request);

            if (result.Id == 0)
                return Unauthorized(new { message = result.Message });

            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
    }

    [Authorize(Policy = "RequireAdministrador")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            await _registerValidator.ValidateAndThrowAsync(request);

            var result = await _authService.RegisterAsync(request);

            if (result.Id == 0)
                return Conflict(new { message = result.Message });

            return CreatedAtAction(null, result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var jti = User.Claims
            .FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti)?.Value;

        if (string.IsNullOrEmpty(jti))
            return Unauthorized(new { message = "Token inválido" });

        await _authService.LogoutAsync(jti);
        return Ok(new { message = "Logout realizado com sucesso" });
    }

    [HttpPost("check")]
    public async Task<IActionResult> Check()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return Ok(new CheckTokenResponse(false));

        var token = authHeader["Bearer ".Length..];
        var result = await _authService.CheckTokenAsync(token);

        return Ok(result);
    }
}
