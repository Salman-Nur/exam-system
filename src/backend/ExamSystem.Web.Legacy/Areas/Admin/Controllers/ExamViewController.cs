using System.Globalization;
using System.Linq.Dynamic.Core;
using Bogus;
using ExamSystem.Infrastructure.Extensions;
using ExamSystem.Web.Data.Entities;
using ExamSystem.Web.Dto;
using ExamSystem.Web.Others;
using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExamViewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Getdata([FromBody] TabulatorQueryDto dto)
        {
            var initialDataSet = GenerateData(500);
            var queryable = initialDataSet.AsQueryable();
            var count = 0;

            IQueryable<Exam>? filteredData = null;

            if (dto.Filters.Count > 0)
            {
                var expression = ExpressionMaker(new[] { "id", "title", "start", "end", "score", "participant", "type" }, dto.Filters);
                filteredData = queryable.Where(expression);
                count = filteredData.Count();
            }

            if (dto.Sorters.Count > 0)
            {
                var elem = dto.Sorters[0];
                var expression = $"x => {elem.Field.Pascalize()} {elem.Dir.ToUpper()}";

                if (filteredData is null)
                {
                    filteredData = queryable.OrderBy(expression);
                    count = queryable.Count();
                }
                else
                {
                    filteredData = filteredData.OrderBy(expression);
                }
            }

            if (filteredData is null)
            {
                filteredData = queryable.Paginate(dto.Page, dto.Size);
                count = queryable.Count();
            }
            else
            {
                filteredData = filteredData.Paginate(dto.Page, dto.Size);
            }

            var totalPages = (int)Math.Ceiling(count / (decimal)dto.Size);

            return Ok(new { data = filteredData, last_row = count, last_page = totalPages });
        }

        private static string ExpressionMaker(string[] allowedColumns, IList<TabulatorFilterDto> filters)
        {
            var expressions = filters
                .Where(filter => allowedColumns.Contains(filter.Field))
                .Select(filter =>
                {
                    var propertyName = filter.Field.Pascalize();
                    var propertyValue = filter.Value;

                    if (propertyName.Equals("Start", StringComparison.OrdinalIgnoreCase) ||
                        propertyName.Equals("End", StringComparison.OrdinalIgnoreCase))
                    {
                        if (DateTime.TryParseExact(propertyValue, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                        {
                            propertyValue = parsedDate.ToString("yyyy-MM-ddTHH:mm:ss");
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid date format for property {propertyName}: {propertyValue}");
                        }
                    }

                    if (filter.Type == FilterHelper.Like)
                    {
                        return $"{propertyName}.Contains(\"{propertyValue}\")";
                    }
                    else if (filter.Type == FilterHelper.Equal)
                    {
                        return $"{propertyName} == \"{propertyValue}\"";
                    }
                    else
                    {
                        return $"{propertyName} {filter.Type} \"{propertyValue}\"";
                    }
                });
            return "x => " + string.Join(" && ", expressions);
        }

        public List<Exam> GenerateData(int count)
        {
            var faker = new Faker<Exam>()
                   .RuleFor(e => e.Id, f => f.Random.Guid())
                   .RuleFor(e => e.Title, f => f.Lorem.Sentence(3))
                   .RuleFor(e => e.Start, f => f.Date.Future())
                   .RuleFor(e => e.End, (f, e) => f.Date.Between(e.Start, e.Start.AddDays(1)))
                   .RuleFor(e => e.Score, f => f.Random.Int(0, 100))
                   .RuleFor(e => e.Participant, f => f.Random.Int(1, 50))
                   .RuleFor(e => e.Type, f => f.PickRandom("Mcq Question", "Short Question", "Coding"));

            return faker.Generate(count);
        }

        public IActionResult ViewItem(int id)
        {
            return View();
        }

        public IActionResult UpdateItem(int id)
        {
            return View();
        }

        public IActionResult DeleteItem(int id)
        {
            //format to show is success with sweet alert , should be sent like this

            return Json(new { success = false, message = "Item not found." });

            // return Json(new { success = true, message = "Item deleted successfully." });

            //return Json(new { success = false, message = "An error occurred while deleting the item." });
        }
    }
}
