using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using ApplicantAdmission.BusinessLogic.Exceptions;

namespace ApplicantAdmission.WebApi.Auth;

public static class UserClaimsExtensions
{
    public static Guid GetUserIdOrThrow(this ClaimsPrincipal user)
    {
        var idStr =
            user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            user.FindFirst("userId")?.Value;

        if (!Guid.TryParse(idStr, out var userId))
            throw new UnauthorizedException("Invalid token: user id missing.");

        return userId;
    }

    public static string GetRoleOrThrow(this ClaimsPrincipal user)
    {
        var role =
            user.FindFirst(ClaimTypes.Role)?.Value ??
            user.FindFirst("role")?.Value;

        if (string.IsNullOrWhiteSpace(role))
            throw new UnauthorizedException("Invalid token: role missing.");

        return role;
    }
}
