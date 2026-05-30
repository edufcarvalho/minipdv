namespace minipdv.Application.DTOs.Auth;

public record LoginRequest(string Login, string Password, string? DeviceInfo = null);
