namespace minipdv.Application.DTOs.Auth;

public record RegisterRequest(string Nome, string Login, string Password, string Tipo = "Usuario", string? Crf = null);
