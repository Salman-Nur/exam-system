using ExamSystem.Application.TagFeature.DataTransferObjects;
using ExamSystem.Application.TagFeature.Service;
using ExamSystem.Infrastructure;

namespace ExamSystem.HttpApi.RequestHandlers
{
    public class GetTagsHandler
    {
        public Task<(IList<TagListDTO> data, int total, int totalDisplay)> GetTagsAsync(IServiceProvider serviceProvider, int pageIndex, int pageSize)
        {
            var tagManagementService = serviceProvider.GetRequiredService<ITagManagementService>();
            return tagManagementService.GetTagsAsync(pageIndex, pageSize);
        }

        public Task<IList<TagDto>> GetAllTags(IServiceProvider serviceProvider)
        {
            var tagManagementService = serviceProvider.GetRequiredService<ITagManagementService>();
            return tagManagementService.GetAllTags();
        }
    }
}
