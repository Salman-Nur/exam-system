using ExamSystem.Application.Common.Extensions;
using ExamSystem.Application.MembershipFeatures.DataTransferObjects;
using ExamSystem.Application.MembershipFeatures.Services;
using ExamSystem.Web.Exceptions;
using ExamSystem.Web.Models.Utilities.TabulatorUtilities;

namespace ExamSystem.Web.Areas.Admin.Models;

public class MemberModel(IMembershipService membershipService)
{
    public async Task<TabulatorResponse<MemberListDTO>> GetMembersAsync(
        TabulatorRequest request)
    {
        var searchRequest = request.GetSearchRequest();
        var validator =
            await new SearchRequestValidator<Domain.Entities.UnaryAggregateRoots.Member>().ValidateAsync(
                searchRequest);

        if (validator.IsValid is false)
        {
            throw new SearchRequestInvalidException(validator.GetErrorMessages());
        }

        var response = await membershipService.GetMembersAsync(searchRequest);
        return new TabulatorResponse<MemberListDTO>(response.Total, response.Size, response.Items.ToList());
    }
}
