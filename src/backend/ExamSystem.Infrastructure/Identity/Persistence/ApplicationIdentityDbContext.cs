using ExamSystem.Infrastructure.Identity.Managers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Infrastructure.Identity.Persistence;

public class ApplicationIdentityDbContext : IdentityDbContext<
    ApplicationIdentityUser, ApplicationRole, Guid,
    ApplicationUserClaim, ApplicationUserRole,
    ApplicationUserLogin, ApplicationRoleClaim,
    ApplicationUserToken>
{
    public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
        : base(options)
    {
    }
}
