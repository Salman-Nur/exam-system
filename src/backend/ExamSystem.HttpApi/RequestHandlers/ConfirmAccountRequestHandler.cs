using ExamSystem.Application.MembershipFeatures.DataTransferObjects;
using ExamSystem.Infrastructure.Identity.Managers;
using SharpOutcome;
using SharpOutcome.Helpers;

namespace ExamSystem.HttpApi.RequestHandlers;

public class ConfirmAccountRequestHandler
{
    public async Task<ValueOutcome<ApplicationIdentityUser, IBadOutcome>> ConductConfirmationAsync(
        IServiceProvider serviceProvider, MemberConfirmAccountRequest dto)
    {
        var applicationUserManager = serviceProvider.GetRequiredService<ApplicationUserManager>();

        var user = await applicationUserManager.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            return new BadOutcome(BadOutcomeTag.NotFound, "User not found");
        }

        if (user.EmailConfirmed)
        {
            return new BadOutcome(BadOutcomeTag.Repetitive, "Already verified.");
        }

        var result = await applicationUserManager.ConfirmEmailAsync(user, dto.Code);

        if (result.Succeeded is false)
        {
            return new BadOutcome(BadOutcomeTag.Invalid, "Invalid verification code.");
        }

        return user;
    }
}
