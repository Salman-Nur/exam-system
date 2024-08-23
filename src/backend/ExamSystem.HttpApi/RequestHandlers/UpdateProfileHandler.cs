using ExamSystem.Application.MembershipFeatures.DataTransferObjects;
using ExamSystem.Application.MembershipFeatures.Services;
using ExamSystem.Infrastructure.Identity.Managers;

namespace ExamSystem.HttpApi.RequestHandlers
{
    public class UpdateProfileHandler
    {
        public async Task<MemberUpdateDTO> GetMemberUserByEmailAsync(IServiceProvider serviceProvider)
        {
            var signInManager = serviceProvider.GetRequiredService<ApplicationSignInManager>();
            var memberManagementService = serviceProvider.GetRequiredService<IMembershipService>();
            var email = signInManager.Context.User.Identity?.Name;

            if (email == null)
            {
                email = "test1@gmail.com";
            }

            var memberUpdateDto = await memberManagementService.GetMemberUserByEmailAsync(email);
            return memberUpdateDto;
        }

        public async Task<string?> UpdateMemberInformationAsync(IServiceProvider serviceProvider, MemberUpdateDTO memberUpdateDto)
        {
            var signInManager = serviceProvider.GetRequiredService<ApplicationSignInManager>();
            var memberManagementService = serviceProvider.GetRequiredService<IMembershipService>();
            var email = signInManager.Context.User.Identity?.Name;

            if (email == null)
            {
                email = "test1@gmail.com";
            }

            var response = await memberManagementService.UpdateMemberInformationAsync(memberUpdateDto, email);
            if (response.IsBadOutcome())
            {
                return "Profile is not updated!";
            }
            return null;
        }
    }
}
