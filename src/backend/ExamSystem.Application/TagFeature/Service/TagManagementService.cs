using ExamSystem.Application.Common.Contracts;
using Mapster;
using SharpOutcome.Helpers;
using SharpOutcome;
using ExamSystem.Application.TagFeature.Enum;
using ExamSystem.Application.TagFeature.DataTransferObjects;
using ExamSystem.Domain.Entities.UnaryAggregateRoots;
using ExamSystem.Application.Common.Providers;

namespace ExamSystem.Application.TagFeature.Service
{
    public class TagManagementService : ITagManagementService
    {
        private readonly IApplicationUnitOfWork _appUnitOfWork;
        private readonly IGuidProvider _guidProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TagManagementService(IApplicationUnitOfWork appUnitOfWork, IGuidProvider guidProvider, IDateTimeProvider dateTimeProvider)
        {
            _appUnitOfWork = appUnitOfWork;
            _guidProvider = guidProvider;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<(IList<TagListDTO> data, int total, int totalDisplay)> GetTagsAsync(int pageIndex, int pageSize)
        {
            var tagEntityList = await _appUnitOfWork.TagRepository.GetTagsAsync(pageIndex, pageSize);
            var result = await tagEntityList.data.BuildAdapter().AdaptToTypeAsync<List<TagListDTO>>();
            return (result, tagEntityList.total, tagEntityList.totalDisplay);
        }

        public async Task<ValueOutcome<string, TagOperationsErrorReason>> CreateTagAsync(TagCreateDTO tagCreateDto)
        {
            var response = await _appUnitOfWork.TagRepository.GetTagByNameAsync(tagCreateDto.Name);
            if (response is not null)
            {
                return TagOperationsErrorReason.DuplicateTag;
            }

            var tag = new Tag()
            {
                Id = _guidProvider.SortableGuid(),
                Name = tagCreateDto.Name,
                CreatedAtUtc = _dateTimeProvider.CurrentUtcTime
            };

            await _appUnitOfWork.TagRepository.CreateTagAsync(tag);
            await _appUnitOfWork.SaveAsync();
            return tag.Id.ToString();
        }

        public async Task<ValueOutcome<TagListDTO, TagOperationsErrorReason>> GetTagByIdAsync(Guid id)
        {
            var tagEntity = await _appUnitOfWork.TagRepository.GetTagByIdAsync(id);
            if (tagEntity == null)
                return TagOperationsErrorReason.NotFound;
            var tagEntityDTO = await tagEntity.BuildAdapter().AdaptToTypeAsync<TagListDTO>();
            return tagEntityDTO;
        }

        public async Task<ValueOutcome<Successful, TagOperationsErrorReason>> UpdateTagAsync(TagUpdateDTO tagUpdateDTO, TagListDTO tagDTO)
        {
            var response = await _appUnitOfWork.TagRepository.GetTagByNameAsync(tagUpdateDTO.Name);
            if (response is not null)
                return TagOperationsErrorReason.DuplicateTag;

            tagDTO.UpdatedAtUtc = _dateTimeProvider.CurrentUtcTime;
            tagDTO.Name = tagUpdateDTO.Name;
            Tag tagEntityObj = await tagDTO.BuildAdapter().AdaptToTypeAsync<Tag>();
            await _appUnitOfWork.TagRepository.UpdateTagAsync(tagEntityObj);
            await _appUnitOfWork.SaveAsync();
            return new Successful();
        }

        public async Task RemoveTagByIdAsync(Guid id)
        {
            await _appUnitOfWork.TagRepository.RemoveTagByIdAsync(id);
            await _appUnitOfWork.SaveAsync();
        }

        public async Task<IList<TagDto>> GetAllTags()
        {
            var tags = await _appUnitOfWork.TagRepository.GetAllTags();
            return await tags.BuildAdapter().AdaptToTypeAsync<IList<TagDto>>();
        }
    }
}
