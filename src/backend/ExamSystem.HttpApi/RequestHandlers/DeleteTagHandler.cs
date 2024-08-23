using ExamSystem.Application.TagFeature.DataTransferObjects;
using ExamSystem.Application.TagFeature.Service;

namespace ExamSystem.HttpApi.RequestHandlers
{
    public class DeleteTagHandler
    {
        public async Task RemoveTagByIdAsync(IServiceProvider serviceProvider,Guid id)
        {
            var tagManagementService = serviceProvider.GetRequiredService<ITagManagementService>();
            await tagManagementService.RemoveTagByIdAsync(id);
        }
    }
}
