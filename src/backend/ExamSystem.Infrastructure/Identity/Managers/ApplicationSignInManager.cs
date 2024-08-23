using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExamSystem.Infrastructure.Identity.Managers;

public class ApplicationSignInManager
    : SignInManager<ApplicationIdentityUser>
{
    public ApplicationSignInManager(UserManager<ApplicationIdentityUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<ApplicationIdentityUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<ApplicationIdentityUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<ApplicationIdentityUser> userConfirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, userConfirmation)
    {
    }
}
