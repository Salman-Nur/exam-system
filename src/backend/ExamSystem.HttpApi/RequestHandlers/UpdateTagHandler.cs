using ExamSystem.Application.TagFeature.DataTransferObjects;
using ExamSystem.Application.TagFeature.Enum;
using ExamSystem.Application.TagFeature.Service;
using SharpOutcome;

namespace ExamSystem.HttpApi.RequestHandlers
{
    public class UpdateTagHandler
    {
        public async Task<ValueOutcome<TagListDTO, string>> GetTagByIdAsync(IServiceProvider serviceProvider, Guid id)
        {
            var tagManagementService = serviceProvider.GetRequiredService<ITagManagementService>();
            var response = await tagManagementService.GetTagByIdAsync(id);
            if (response.IsBadOutcome())
            {
                return "This tag doesn't exist anymore !!";
            }
            response.TryPickGoodOutcome(out var tagDTO);
            return tagDTO;
        }

        public async Task<string?> UpdateTagAsync(IServiceProvider serviceProvider, TagUpdateDTO tagUpdateDTO, TagListDTO tagDTO)
        {
            var tagManagementService = serviceProvider.GetRequiredService<ITagManagementService>();
            var response = await tagManagementService.UpdateTagAsync(tagUpdateDTO, tagDTO);
            if (response.IsBadOutcome())
            {
                return "Tag is not Updated! Duplicate tag found";
            }
            return null;
        }
    }
}
