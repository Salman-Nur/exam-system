using ExamSystem.Domain.Entities.UnaryAggregateRoots;

namespace ExamSystem.Domain.Repositories;

public interface ITagRepository : IRepositoryBase<Tag, Guid>
{
    Task<(IList<Tag> data, int total, int totalDisplay)> GetTagsAsync(int pageIndex, int pageSize);
    Task<Tag?> GetTagByNameAsync(string name);
    Task CreateTagAsync(Tag tagEntity);
    Task<Tag> GetTagByIdAsync(Guid id);
    Task UpdateTagAsync(Tag tagUpdateEntity);
    Task RemoveTagByIdAsync(Guid id);
    Task<IList<Tag>> GetAllTags();
}
