using System.Linq.Dynamic.Core;
using System.Text;
using ExamSystem.Infrastructure.Extensions;
using ExamSystem.Web.Areas.Admin.Models;
using ExamSystem.Web.Dto;
using ExamSystem.Web.Others;
using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LogController : Controller
    {
        private readonly LinkGenerator _linkGenerator;
        private static List<LogEntryModel> _logEntries;

        public LogController(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }
        public IActionResult Index()
        {
            _logEntries = new List<LogEntryModel>
            {
                new LogEntryModel { Id = 1, Message = "System started 1", Level = LogLevels.Information, Time = DateTime.Now.AddMinutes(-10) },
                new LogEntryModel { Id = 2, Message = "User logged in 1", Level = LogLevels.Information, Time = DateTime.Now.AddMinutes(-5) },
                new LogEntryModel { Id = 3, Message = "Error occurred 1", Level = LogLevels.Error, Time = DateTime.Now },
                new LogEntryModel { Id = 4, Message = "System started 2", Level = LogLevels.Information, Time = DateTime.Now.AddMinutes(-10) },
                new LogEntryModel { Id = 5, Message = "User logged in 2", Level = LogLevels.Information, Time = DateTime.Now.AddMinutes(-5) },
                new LogEntryModel { Id = 6, Message = "Error occurred 2", Level = LogLevels.Error, Time = DateTime.Now },
                new LogEntryModel { Id = 7, Message = "System started 3", Level = LogLevels.Information, Time = DateTime.Now.AddMinutes(-10) },
                new LogEntryModel { Id = 8, Message = "User logged in 3", Level = LogLevels.Information, Time = DateTime.Now.AddMinutes(-5) },
                new LogEntryModel { Id = 9, Message = "Error occurred 3", Level = LogLevels.Error, Time = DateTime.Now },
                new LogEntryModel { Id = 10, Message = "System started 4", Level = LogLevels.Information, Time = DateTime.Now.AddMinutes(-10) },
                new LogEntryModel { Id = 12, Message = "User logged in 4", Level = LogLevels.Information, Time = DateTime.Now.AddMinutes(-5) },
                new LogEntryModel { Id = 13, Message = "Error occurred 4", Level = LogLevels.Error, Time = DateTime.Now },
                new LogEntryModel { Id = 11, Message = "System started 5", Level = LogLevels.Information, Time = DateTime.Now.AddMinutes(-10) },
                new LogEntryModel { Id = 14, Message = "User logged in 5", Level = LogLevels.Information, Time = DateTime.Now.AddMinutes(-5) },
                new LogEntryModel { Id = 15, Message = "Error occurred 5", Level = LogLevels.Error, Time = DateTime.Now },
                new LogEntryModel { Id = 16, Message = "System started 6", Level = LogLevels.Information, Time = DateTime.Now.AddMinutes(-10) },
                new LogEntryModel { Id = 17, Message = "User logged in 6", Level = LogLevels.Information, Time = DateTime.Now.AddMinutes(-5) },
                new LogEntryModel { Id = 18, Message = "Error occurred 6", Level = LogLevels.Error, Time = DateTime.Now },
                new LogEntryModel { Id = 19, Message = "System started 7", Level = LogLevels.Information, Time = DateTime.Now.AddMinutes(-10) },
                new LogEntryModel { Id = 20, Message = "User logged in 7", Level = LogLevels.Information, Time = DateTime.Now.AddMinutes(-5) },
                new LogEntryModel { Id = 23, Message = "Error occurred 7", Level = LogLevels.Error, Time = DateTime.Now },
                new LogEntryModel { Id = 21, Message = "System started 8", Level = LogLevels.Information, Time = DateTime.Now.AddMinutes(-10) },
                new LogEntryModel { Id = 22, Message = "User logged in 8", Level = LogLevels.Information, Time = DateTime.Now.AddMinutes(-5) },
                new LogEntryModel { Id = 24, Message = "Error occurred 8", Level = LogLevels.Error, Time = DateTime.Now }
            };
            var url = _linkGenerator.GetUriByAction(HttpContext, controller: "Log", action: "GetLogEntries");
            if (url is null)
            {
                return View();
            }
            var viewModel = new TabularViewModel { EndpointUrl = url };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult GetLogEntries([FromBody] TabulatorQueryDto dto)
        {
            var initialDataSet = _logEntries;
            var queryable = initialDataSet.AsQueryable();
            var count = 0;

            IQueryable<LogEntryModel>? filteredData = null;

            if (dto.Filters.Count > 0)
            {
                var expression = ExpressionMaker(
                    ["id", "message", "level", "time", "levelName"],
                    ["level"],
                    dto.Filters
                );
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

        private static string ExpressionMaker(IList<string> allowedColumns, IList<string> enumColumns, IList<TabulatorFilterDto> filters)
        {
            var expression = new StringBuilder();
            expression.Append("x => ");

            for (var i = 0; i < filters.Count; i++)
            {
                var filter = filters[i];
                if (filter.Field == "levelName")
                {
                    filter = new TabulatorFilterDto("level", "=", filter.Value);
                }
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
                else if (fieldInPascalCase == "Time")
                {
                    var dateTimeValue = DateTime.ParseExact(filter.Value, "dd/MM/yyyy HH:mm", null);
                    expression.Append($"{fieldInPascalCase} {filter.Type} DateTime({dateTimeValue.Year}, {dateTimeValue.Month}, {dateTimeValue.Day}, {dateTimeValue.Hour}, {dateTimeValue.Minute}, {dateTimeValue.Second})");
                }
                else
                {
                    expression.Append($"{fieldInPascalCase} {filter.Type} {filter.Value}");
                }

                if (i + 1 != filters.Count)
                {
                    expression.Append(" && ");
                }
            }

            return expression.ToString();
        }
    }
}
