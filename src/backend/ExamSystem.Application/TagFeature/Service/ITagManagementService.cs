using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExamSystem.Application.TagFeature.DataTransferObjects;
using ExamSystem.Application.TagFeature.Enum;
using SharpOutcome.Helpers;
using SharpOutcome;

namespace ExamSystem.Application.TagFeature.Service
{
    public interface ITagManagementService
    {
        Task<(IList<TagListDTO> data, int total, int totalDisplay)> GetTagsAsync(int pageIndex, int pageSize);
        Task<ValueOutcome<string, TagOperationsErrorReason>> CreateTagAsync(TagCreateDTO tagCreateDto);
        Task<ValueOutcome<TagListDTO, TagOperationsErrorReason>> GetTagByIdAsync(Guid id);
        Task<ValueOutcome<Successful, TagOperationsErrorReason>> UpdateTagAsync(TagUpdateDTO tagUpdateDTO, TagListDTO tagDTO);
        Task RemoveTagByIdAsync(Guid id);
        Task<IList<TagDto>> GetAllTags();
    }
}
