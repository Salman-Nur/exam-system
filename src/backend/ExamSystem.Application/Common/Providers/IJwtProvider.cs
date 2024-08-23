using System.Security.Claims;

namespace ExamSystem.Application.Common.Providers;

public interface IJwtProvider
{
    (string token, DateTime duration) GenerateJwt(Dictionary<string, object> claims);
    (string token, DateTime duration) GenerateJwt(IEnumerable<Claim> claims);
}
