using System.Security.Claims;
using System.Text;
using ExamSystem.Application.Common.Options;
using ExamSystem.Application.Common.Providers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ExamSystem.Infrastructure.Providers;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly IDateTimeProvider _dateTimeProvider;

    public JwtProvider(IOptions<JwtOptions> jwtOptions, IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        _jwtOptions = jwtOptions.Value;
    }

    public (string token, DateTime duration) GenerateJwt(Dictionary<string, object> claims)
    {
        ArgumentNullException.ThrowIfNull(claims);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenDuration = _dateTimeProvider.CurrentUtcTime.AddMinutes(_jwtOptions.ExpiryMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            IssuedAt = _dateTimeProvider.CurrentUtcTime,
            Expires = tokenDuration,
            SigningCredentials = credentials
        };

        if (claims.Count > 0)
        {
            descriptor.Claims = claims;
        }

        var handler = new JsonWebTokenHandler { SetDefaultTimesOnTokenCreation = false };
        return (handler.CreateToken(descriptor), tokenDuration);
    }

    public (string token, DateTime duration) GenerateJwt(IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims);
        Dictionary<string, object> storage = [];

        foreach (var claim in claims)
        {
            storage.Add(claim.Type, claim.Value);
        }

        return GenerateJwt(storage);
    }
}
