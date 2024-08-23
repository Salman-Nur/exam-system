using ExamSystem.Web.Data.Entities;

namespace ExamSystem.Web.Others;

public static class FakeBulkData
{
    public static readonly List<Book> BookData = GenerateBookData(500).Result;

    public static async Task<List<Book>> GenerateBookData(int amountOfData)
    {
        List<string> genres = ["Fantasy", "Sci-Fi", "Mystery", "Romance", "Horror", "Non-fiction"];

        return await Task.Run(() =>
        {
            return new Bogus.Faker<Book>()
                .RuleFor(b => b.Id, f => f.IndexFaker + 1)
                .RuleFor(b => b.InventoryStatus, f => f.PickRandom<InventoryStatus>())
                .RuleFor(b => b.Title, f => f.Lorem.Sentence(3))
                .RuleFor(b => b.Genre, f => f.PickRandom(genres))
                .RuleFor(b => b.Author, f => f.Name.FullName())
                .RuleFor(b => b.Price, f => f.Random.Int(100, 1000))
                .Generate(amountOfData);
        });
    }
}
