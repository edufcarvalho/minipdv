namespace minipdv.Application.DTOs.Auth;

public record CheckTokenResponse(bool Authenticated, string? Nome = null, string? Login = null);
