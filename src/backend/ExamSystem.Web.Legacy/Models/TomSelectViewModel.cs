using ExamSystem.Web.Data.Entities;

namespace ExamSystem.Web.Models;

public class TomSelectViewModel
{
    public HashSet<Guid> SelectedExistingTagIdentifiers { get; set; } = [];
    public HashSet<string> NewCreatedTags { get; set; } = [];
    public byte MaxTags { get; set; } = 15;
    public bool CanCreate { get; set; } = true;
    public uint TagMaxLength { get; set; } = 20;

    public async Task<IList<Tag>> FetchTagsAsync()
    {
        return await Task.Run(() =>
        {
            return new Bogus.Faker<Tag>()
                .RuleFor(b => b.Id, _ => Guid.NewGuid())
                .RuleFor(b => b.Title, f => f.Lorem.Word())
                .Generate(5);
        });
    }
}
