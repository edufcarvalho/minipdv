using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace minipdv.Infrastructure.Security;

public static class PasswordHasher
{
    private static readonly PasswordHasher<object> _hasher = new(Options.Create(new PasswordHasherOptions()));

    public static string Hash(string password)
    {
        return _hasher.HashPassword(null!, password);
    }

    public static bool Verify(string password, string hash)
    {
        var result = _hasher.VerifyHashedPassword(null!, hash, password);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
