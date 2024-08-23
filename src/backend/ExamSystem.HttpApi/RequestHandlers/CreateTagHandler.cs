using ExamSystem.Application.TagFeature.DataTransferObjects;
using ExamSystem.Application.TagFeature.Service;
using SharpOutcome;

namespace ExamSystem.HttpApi.RequestHandlers
{
    public class CreateTagHandler
    {
        public static async Task<ValueOutcome<string, bool>> CreateTagAsync(IServiceProvider serviceProvider, TagCreateDTO tagCreateDTO)
        {
            var tagManagementService = serviceProvider.GetRequiredService<ITagManagementService>();
            var response = await tagManagementService.CreateTagAsync(tagCreateDTO);

            if (response.TryPickGoodOutcome(out var id))
            {
                return id;
            }
            return false;
        }
    }
}
