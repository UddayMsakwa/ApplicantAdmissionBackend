using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ApplicantAdmission.BusinessLogic.Services;

public static class PasswordHasher
{
    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);

        var hash = KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32);

        return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
    }

    public static bool Verify(string password, string stored)
    {
        var parts = stored.Split('.');
        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);

        var computed = KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA256,
            100_000,
            32);

        return CryptographicOperations.FixedTimeEquals(hash, computed);
    }
}
