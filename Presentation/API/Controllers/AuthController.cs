using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minipdv.Application.DTOs.Auth;
using minipdv.Application.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly MiniPDVContext _context;

    public AuthController(IAuthService authService, MiniPDVContext context)
    {
        _authService = authService;
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (result.Id == 0)
            return Unauthorized(new { message = result.Message });

        await _context.SaveChangesAsync();
        return Ok(result);
    }

    [Authorize(Policy = Policies.RequireAdministrador)]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        if (result.Id == 0)
            return Conflict(new { message = result.Message });

        await _context.SaveChangesAsync();
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

        await _authService.LogoutAsync(jti);
        await _context.SaveChangesAsync();
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
