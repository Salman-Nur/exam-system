using System.Security.Claims;
using ExamSystem.Application.Common.Providers;
using ExamSystem.Application.MembershipFeatures.DataTransferObjects;
using ExamSystem.Infrastructure.Identity.Managers;
using SharpOutcome;
using SharpOutcome.Helpers;

namespace ExamSystem.HttpApi.RequestHandlers;

public class LoginRequestHandler
{
    public async
        Task<ValueOutcome<(string token, DateTime duration, CookieOptions cookieOptions), BadOutcomeTag>>
        HandleAsync(IServiceProvider serviceProvider, MemberLoginRequest dto, CancellationToken ct)
    {
        var userManager = serviceProvider.GetRequiredService<ApplicationUserManager>();

        var entity = await userManager.FindByEmailAsync(dto.Email);

        if (entity is null)
        {
            return BadOutcomeTag.NotFound;
        }

        var signInManager = serviceProvider.GetRequiredService<ApplicationSignInManager>();
        var result = await signInManager.CheckPasswordSignInAsync(entity, dto.Password, true);

        if (result.Succeeded is false)
        {
            return result.IsNotAllowed ? BadOutcomeTag.NotVerified : BadOutcomeTag.Unauthorized;
        }

        var claims = await userManager.GetClaimsAsync(entity);
        claims.Add(new Claim(ClaimTypes.Name, entity.UserName.ToString()));
        claims.Add(new Claim(ClaimTypes.NameIdentifier, entity.Id.ToString()));
        claims.Add(new Claim(ClaimTypes.Email, dto.Email));

        var jwtProvider = serviceProvider.GetRequiredService<IJwtProvider>();
        var tokenData = jwtProvider.GenerateJwt(claims);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, Expires = dto.RememberMe ? tokenData.duration : null
        };

        return (tokenData.token, tokenData.duration, cookieOptions);
    }
}
