using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace minipdv.Infrastructure.Security;

public enum PasswordVerificationResult
{
    Success,
    Failed,
    SuccessRehashNeeded
}

public static class PasswordHasher
{
    private static readonly PasswordHasher<object> _hasher = new(Options.Create(new PasswordHasherOptions()));

    public static string Hash(string password)
    {
        return _hasher.HashPassword(null!, password);
    }

    public static bool Verify(string password, string hash)
    {
        var result = VerifyWithResult(password, hash);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }

    public static PasswordVerificationResult VerifyWithResult(string password, string hash)
    {
        var result = _hasher.VerifyHashedPassword(null!, hash, password);
        return result switch
        {
            Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success => PasswordVerificationResult.Success,
            Microsoft.AspNetCore.Identity.PasswordVerificationResult.SuccessRehashNeeded => PasswordVerificationResult.SuccessRehashNeeded,
            _ => PasswordVerificationResult.Failed
        };
    }

    public static string? RehashIfNeeded(string password, string hash)
    {
        var result = _hasher.VerifyHashedPassword(null!, hash, password);
        if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.SuccessRehashNeeded)
            return Hash(password);
        return null;
    }
}
