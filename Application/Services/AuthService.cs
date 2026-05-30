using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using minipdv.Application.DTOs.Auth;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Configuration;
using minipdv.Infrastructure.Security;

namespace minipdv.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ISessionRepository _sessionRepository;

    public AuthService(IUsuarioRepository usuarioRepository, ISessionRepository sessionRepository)
    {
        _usuarioRepository = usuarioRepository;
        _sessionRepository = sessionRepository;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var usuario = await _usuarioRepository.GetByLoginAsync(request.Login);

        if (usuario is null || !PasswordHasher.Verify(request.Password, usuario.PasswordHash))
            return new AuthResponse(0, string.Empty, string.Empty, string.Empty, "Login ou senha inválidos");

        if (!usuario.Ativo)
            return new AuthResponse(0, string.Empty, string.Empty, string.Empty, "Usuário inativo");

        var token = Guid.NewGuid().ToString();
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var session = new Session
        {
            UsuarioId = usuario.Id,
            Token = token,
            DeviceInfo = request.DeviceInfo ?? "Desconhecido",
            ExpiresAt = expiresAt,
            IsRevoked = false
        };

        await _sessionRepository.AddAsync(session);

        var jwt = GenerateJwt(usuario.Id, usuario.Nome, token, expiresAt);

        return new AuthResponse(usuario.Id, usuario.Nome, usuario.Login, jwt, "Login realizado com sucesso");
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existing = await _usuarioRepository.GetByLoginAsync(request.Login);
        if (existing is not null)
            return new AuthResponse(0, string.Empty, string.Empty, string.Empty, "Login já está em uso");

        var usuario = new Usuario
        {
            Nome = request.Nome,
            Login = request.Login,
            PasswordHash = PasswordHasher.Hash(request.Password)
        };

        await _usuarioRepository.AddAsync(usuario);

        return new AuthResponse(usuario.Id, usuario.Nome, usuario.Login, string.Empty, "Usuário registrado com sucesso");
    }

    public async Task LogoutAsync(string jti)
    {
        var session = await _sessionRepository.GetByTokenAsync(jti);
        if (session is not null)
        {
            await _sessionRepository.RevokeAsync(session.Id);
        }
    }

    private static string GenerateJwt(int usuarioId, string nome, string jti, DateTime expiresAt)
    {
        var secret = EnvConfig.Get("JWT_SECRET") ?? "change-in-production-should-be-at-least-32-chars";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuarioId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(JwtRegisteredClaimNames.Name, nome),
        };

        var token = new JwtSecurityToken(
            issuer: "MiniPDV",
            audience: "MiniPDV",
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
