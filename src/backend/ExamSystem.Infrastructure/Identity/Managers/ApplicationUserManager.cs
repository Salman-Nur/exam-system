using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExamSystem.Infrastructure.Identity.Managers;

public class ApplicationUserManager
    : UserManager<ApplicationIdentityUser>
{
    public ApplicationUserManager(IUserStore<ApplicationIdentityUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<ApplicationIdentityUser> passwordHasher,
        IEnumerable<IUserValidator<ApplicationIdentityUser>> userValidators,
        IEnumerable<IPasswordValidator<ApplicationIdentityUser>> passwordValidators,
        ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
        IServiceProvider services, ILogger<UserManager<ApplicationIdentityUser>> logger)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
    }
}
