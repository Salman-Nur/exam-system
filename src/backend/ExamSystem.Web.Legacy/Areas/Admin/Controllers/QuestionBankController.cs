using System.Linq.Dynamic.Core;
using System.Text;
using ExamSystem.Infrastructure.Extensions;
using ExamSystem.Web.Dto;
using ExamSystem.Web.Models;
using ExamSystem.Web.Others;
using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuestionBankController : Controller
    {
        private readonly LinkGenerator _linkGenerator;
        public QuestionBankController(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public IActionResult Index()
        {
            var url = _linkGenerator.GetUriByAction(HttpContext, controller: "QuestionBank", action: "GetQuestions");
            if (url is null)
            {
                return View();
            }

            var viewModel = new TabularViewModel { EndpointUrl = url };
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            return View(id);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetQuestions([FromBody] TabulatorQueryDto dto)
        {
            var initialDataSet = await GenerateTestQuestion();
            var queryable = initialDataSet.AsQueryable();
            var count = 0;

            IQueryable<TestQuestion>? finalData = null;
            IQueryable<TestQuestion>? filteredData = null;
            IOrderedQueryable<TestQuestion>? orderedData = null;
            IOrderedQueryable<TestQuestion>? filteredOrderedData = null;

            if (dto.Filters.Count > 0)
            {
                var expression = ExpressionMaker(
                    ["questionTitle", "difficulty", "tags", "createdAt", "modifiedAt"],
                    ["difficulty"],
                    dto.Filters
                );

                filteredData = queryable.Where(expression);
                var cccccc = filteredData.Count();
            }

            if (dto.Sorters.Count > 0)
            {
                var firstElem = dto.Sorters[0];
                var initialExpression = $"x => {firstElem.Field.Pascalize()} {firstElem.Dir.ToUpper()}";

                if (filteredData is null)
                {
                    orderedData = queryable.OrderBy(initialExpression);
                }

                if (filteredData is not null)
                {
                    filteredOrderedData = queryable.OrderBy(initialExpression);
                }

                if (filteredData is null && dto.Sorters.Count > 1 && orderedData is not null)
                {
                    for (var i = 1; i < dto.Sorters.Count; i++)
                    {
                        var sorter = dto.Sorters[i];
                        var thenExpression = $"x => {sorter.Field.Pascalize()} {sorter.Dir.ToUpper()}";
                        orderedData = orderedData.ThenBy(thenExpression);
                    }
                }

                else if (filteredData is not null && dto.Sorters.Count > 1 && filteredOrderedData is not null)
                {
                    for (var i = 1; i < dto.Sorters.Count; i++)
                    {
                        var sorter = dto.Sorters[i];
                        var thenExpression = $"x => {sorter.Field.Pascalize()} {sorter.Dir.ToUpper()}";
                        filteredOrderedData = filteredOrderedData.ThenBy(thenExpression);
                    }
                }
            }

            if (filteredData is null && orderedData is null && filteredOrderedData is null)
            {
                finalData = queryable.Paginate(dto.Page, dto.Size);
                count = queryable.Count();
            }

            else if (filteredData is not null && orderedData is null && filteredOrderedData is null)
            {
                finalData = filteredData.Paginate(dto.Page, dto.Size);
                count = filteredData.Count();
            }


            else if (filteredData is null && orderedData is not null)
            {
                finalData = orderedData.Paginate(dto.Page, dto.Size);
                count = orderedData.Count();
            }

            else if (filteredData is not null && filteredOrderedData is not null)
            {
                count = filteredOrderedData.Count();
                finalData = filteredOrderedData.Paginate(dto.Page, dto.Size);
            }


            var totalPages = (int)Math.Ceiling(count / (decimal)dto.Size);

            return Ok(new { data = finalData, last_row = count, last_page = totalPages });
        }

        private static string ExpressionMaker(IList<string> allowedColumns, IList<string> enumColumns,
    IList<TabulatorFilterDto> filters)
        {
            var expression = new StringBuilder();
            expression.Append("x => ");

            for (var i = 0; i < filters.Count; i++)
            {
                var filter = filters[i];
                var fieldInPascalCase = filter.Field.Pascalize();

                if (allowedColumns.Contains(filter.Field) is false)
                {
                    continue;
                }

                if (enumColumns.Contains(filter.Field))
                {
                    expression.Append($"{fieldInPascalCase} = {fieldInPascalCase}.{filter.Value.Pascalize()}");
                }
                else if (filter.Type == FilterHelper.Like)
                {
                    expression.Append(
                        $"""{fieldInPascalCase}.Contains("{filter.Value}", StringComparison.InvariantCultureIgnoreCase)"""
                    );
                }
                else if (filter.Field.Pascalize() == "CreatedAt" || filter.Field.Pascalize() == "ModifiedAt")
                {
                    if (filter.Type == FilterHelper.Equal || filter.Type == FilterHelper.Like || filter.Type == FilterHelper.NotEquals)
                    {
                        expression.Append($"x.{filter.Field}.ToString() {filter.Type} \"{filter.Value}\"");
                    }
                    else
                    {
                        expression.Append($"DateTime(x.{filter.Field}.Year, x.{filter.Field}.Month, x.{filter.Field}.Day) {filter.Type} DateTime.Parse(\"{filter.Value}\")");
                    }
                }
                else
                {
                    expression.Append($"{fieldInPascalCase} {filter.Type} \"{filter.Value}\"");
                }

                if (i + 1 != filters.Count)
                {
                    expression.Append(" && ");
                }
            }

            return expression.ToString();
        }

        private static async Task<List<TestQuestion>> GenerateTestQuestion()
        {
            var levels = new List<string> { "Easy", "Medium", "Hard" };
            var tags = new List<string> { "C#", "Asp.Net", "CSS", "HTML" };

            return await Task.Run(() =>
            {
                var levels = Enum.GetValues(typeof(DifficultyLevel));
                return new Bogus.Faker<TestQuestion>()
                    .RuleFor(q => q.Id, f => f.Random.Guid())
                    .RuleFor(q => q.QuestionTitle, f => f.Lorem.Sentence(2))
                    .RuleFor(q => q.Difficulty, f => f.PickRandom((DifficultyLevel[])levels))
                    .RuleFor(q => q.Tags, f => f.PickRandom(tags))
                    .RuleFor(q => q.CreatedAt, f => DateOnly.FromDateTime(f.Date.Past(5)))
                    .RuleFor(q => q.ModifiedAt, (f, q) => DateOnly.FromDateTime(f.Date.Between(q.CreatedAt.ToDateTime(TimeOnly.MinValue), DateTime.Now)))
                    .Generate(500);
            });
        }
    }
    public class TestQuestion
    {
        public Guid Id { get; set; }
        public string QuestionTitle { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public string Tags { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly ModifiedAt { get; set; }
    }

    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }
}
