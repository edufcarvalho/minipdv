using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using minipdv.Application.DTOs.Auth;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Entities.Base;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Configuration;
using minipdv.Infrastructure.Security;

namespace minipdv.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAbstractUsuarioRepository _abstractUsuarioRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IFarmaceuticoRepository _farmaceuticoRepository;
    private readonly IAdministradorRepository _administradorRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly AppSettings _settings;

    public AuthService(
        IAbstractUsuarioRepository abstractUsuarioRepository,
        IUsuarioRepository usuarioRepository,
        IFarmaceuticoRepository farmaceuticoRepository,
        IAdministradorRepository administradorRepository,
        ISessionRepository sessionRepository,
        AppSettings settings)
    {
        _abstractUsuarioRepository = abstractUsuarioRepository;
        _usuarioRepository = usuarioRepository;
        _farmaceuticoRepository = farmaceuticoRepository;
        _administradorRepository = administradorRepository;
        _sessionRepository = sessionRepository;
        _settings = settings;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var usuario = await _abstractUsuarioRepository.GetByLoginAsync(request.Login);

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

        var jwt = GenerateJwt(usuario.Id, usuario.Nome, sessionToken, expiresAt, usuario.TipoUsuario);

        return new AuthResponse(usuario.Id, usuario.Nome, usuario.Login, jwt, "Login realizado com sucesso");
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existing = await _abstractUsuarioRepository.GetByLoginAsync(request.Login);

        if (existing is not null)
            return new AuthResponse(0, string.Empty, string.Empty, string.Empty, "Login já está em uso");

        switch (request.Tipo)
        {
            case "Administrador":
                var admin = new Administrador
                {
                    Nome = request.Nome,
                    Login = request.Login,
                    PasswordHash = PasswordHasher.Hash(request.Password),
                    TipoUsuario = "Administrador"
                };
                await _administradorRepository.AddAsync(admin);
                return new AuthResponse(admin.Id, admin.Nome, admin.Login, string.Empty, "Administrador registrado com sucesso");

            case "Farmaceutico":
                var farm = new Farmaceutico
                {
                    Nome = request.Nome,
                    Login = request.Login,
                    PasswordHash = PasswordHasher.Hash(request.Password),
                    TipoUsuario = "Farmaceutico",
                    Crf = request.Crf ?? throw new ValidationException("CRF é obrigatório para Farmacêutico")
                };
                await _farmaceuticoRepository.AddAsync(farm);
                return new AuthResponse(farm.Id, farm.Nome, farm.Login, string.Empty, "Farmacêutico registrado com sucesso");

            default:
                var user = new Usuario
                {
                    Nome = request.Nome,
                    Login = request.Login,
                    PasswordHash = PasswordHasher.Hash(request.Password),
                    TipoUsuario = "Usuario"
                };
                await _usuarioRepository.AddAsync(user);
                return new AuthResponse(user.Id, user.Nome, user.Login, string.Empty, "Usuário registrado com sucesso");
        }
    }

    public async Task LogoutAsync(string jti)
    {
        var session = await _sessionRepository.GetByTokenAsync(jti);
        if (session is not null)
        {
            await _sessionRepository.RevokeAsync(session.Id);
        }
    }

    public async Task<CheckTokenResponse> CheckTokenAsync(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtSecret));

            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            handler.ValidateToken(token, parameters, out var validatedToken);
            var jwt = (JwtSecurityToken)validatedToken;
            var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (string.IsNullOrEmpty(jti))
                return new CheckTokenResponse(false);

            var session = await _sessionRepository.GetByTokenAsync(jti);
            if (session is null || session.IsRevoked || session.ExpiresAt <= DateTime.UtcNow)
            {
                if (session is not null && !session.IsRevoked)
                    await _sessionRepository.RevokeAsync(session.Id);

                return new CheckTokenResponse(false);
            }

            var nome = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;
            var tipo = jwt.Claims.FirstOrDefault(c => c.Type == "tipo")?.Value;
            return new CheckTokenResponse(true, nome, Tipo: tipo);
        }
        catch
        {
            return new CheckTokenResponse(false);
        }
    }

    private string GenerateJwt(int usuarioId, string nome, string jti, DateTime expiresAt, string tipo)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuarioId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(JwtRegisteredClaimNames.Name, nome),
            new Claim("tipo", tipo),
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
