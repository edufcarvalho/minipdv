using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minipdv.Application.DTOs.Auth;
using minipdv.Application.UseCases.Auth;

namespace minipdv.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;
    private readonly RegisterUseCase _registerUseCase;
    private readonly LogoutUseCase _logoutUseCase;
    private readonly CheckTokenUseCase _checkTokenUseCase;

    public AuthController(
        LoginUseCase loginUseCase,
        RegisterUseCase registerUseCase,
        LogoutUseCase logoutUseCase,
        CheckTokenUseCase checkTokenUseCase)
    {
        _loginUseCase = loginUseCase;
        _registerUseCase = registerUseCase;
        _logoutUseCase = logoutUseCase;
        _checkTokenUseCase = checkTokenUseCase;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _loginUseCase.ExecuteAsync(request);

        if (result.Id == 0)
            return Unauthorized(new { message = result.Message });

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _registerUseCase.ExecuteAsync(request);

        if (result.Id == 0)
            return Conflict(new { message = result.Message });

        return CreatedAtAction(null, result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var jti = User.Claims
            .FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti)?.Value;

        if (string.IsNullOrEmpty(jti))
            return Unauthorized(new { message = "Token inválido" });

        await _logoutUseCase.ExecuteAsync(jti);
        return Ok(new { message = "Logout realizado com sucesso" });
    }

    [HttpPost("check")]
    public async Task<IActionResult> Check()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return Ok(new CheckTokenResponse(false));

        var token = authHeader["Bearer ".Length..];
        var result = await _checkTokenUseCase.ExecuteAsync(token);

        return Ok(result);
    }
}
