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
    private readonly AppSettings _settings;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        ISessionRepository sessionRepository,
        AppSettings settings)
    {
        _usuarioRepository = usuarioRepository;
        _sessionRepository = sessionRepository;
        _settings = settings;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var usuario = await _usuarioRepository.GetByLoginAsync(request.Login);

        if (usuario is null || !PasswordHasher.Verify(request.Password, usuario.PasswordHash))
            return new AuthResponse(0, string.Empty, string.Empty, string.Empty, "Login ou senha inválidos");

        if (!usuario.Ativo)
            return new AuthResponse(0, string.Empty, string.Empty, string.Empty, "Usuário inativo");

        var sessionToken = Guid.NewGuid().ToString();
        var expiresAt = DateTime.UtcNow.AddDays(_settings.JwtExpirationDays);

        var session = new Session
        {
            UsuarioId = usuario.Id,
            Token = sessionToken,
            DeviceInfo = request.DeviceInfo ?? "Desconhecido",
            ExpiresAt = expiresAt,
            IsRevoked = false
        };

        await _sessionRepository.AddAsync(session);

        var jwt = GenerateJwt(usuario.Id, usuario.Nome, sessionToken, expiresAt);

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

    private string GenerateJwt(int usuarioId, string nome, string jti, DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuarioId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(JwtRegisteredClaimNames.Name, nome),
        };

        var token = new JwtSecurityToken(
            issuer: _settings.JwtIssuer,
            audience: _settings.JwtAudience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
