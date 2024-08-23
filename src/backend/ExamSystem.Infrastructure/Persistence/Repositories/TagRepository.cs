using System.Linq.Expressions;
using ExamSystem.Application.TagFeature.DataTransferObjects;
using ExamSystem.Domain.Entities.UnaryAggregateRoots;
using ExamSystem.Domain.Repositories;

namespace ExamSystem.Infrastructure.Persistence.Repositories;

public class TagRepository : Repository<Tag, Guid>, ITagRepository
{
    public TagRepository(ExamSystemDbContext context) : base(context)
    {
    }

    public async Task<(IList<Tag> data, int total, int totalDisplay)> GetTagsAsync(int pageIndex, int pageSize)
    {
        return await GetDynamicAsync(null, null, null, pageIndex, pageSize, true);
    }

    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        Expression<Func<Tag, bool>> expression = x => x.Name == name;
        return await GetOneAsync(expression);
    }

    public async Task CreateTagAsync(Tag tagEntity)
    {
        await CreateAsync(tagEntity);
    }

    public async Task<Tag?> GetTagByIdAsync(Guid id)
    {
        Expression<Func<Tag, bool>> expression = x => x.Id == id;
        return await GetOneAsync(expression,false,null,true);
    }
    public async Task UpdateTagAsync(Tag tagUpdateEntity)
    {
        await UpdateAsync(tagUpdateEntity);
    }

    public async Task RemoveTagByIdAsync(Guid id)
    {
        Expression<Func<Tag, bool>> expression = x => x.Id == id;
        await RemoveAsync(expression);
    }

    public Task<IList<Tag>> GetAllTags()
    {
        return GetAsync(null, null);
    }
}
