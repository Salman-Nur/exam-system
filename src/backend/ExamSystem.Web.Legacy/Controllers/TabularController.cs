using System.Linq.Dynamic.Core;
using System.Text;
using ExamSystem.Infrastructure.Extensions;
using ExamSystem.Web.Data.Entities;
using ExamSystem.Web.Dto;
using ExamSystem.Web.Models;
using ExamSystem.Web.Others;
using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Web.Controllers;

public class TabularController : Controller
{
    private readonly LinkGenerator _linkGenerator;

    public TabularController(LinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }

    public IActionResult Index()
    {
        var url = _linkGenerator.GetUriByAction(HttpContext, controller: "Tabular", action: "GetBooks");
        if (url is null)
        {
            return View();
        }

        var viewModel = new TabularViewModel { EndpointUrl = url };
        return View(viewModel);
    }


    [HttpPost]
    public ActionResult GetBooks([FromBody] TabulatorQueryDto dto)
    {
        var initialDataSet = FakeBulkData.BookData;
        var queryable = initialDataSet.AsQueryable();
        var count = 0;

        IQueryable<Book>? finalData = null;
        IQueryable<Book>? filteredData = null;
        IOrderedQueryable<Book>? orderedData = null;
        IOrderedQueryable<Book>? filteredOrderedData = null;

        if (dto.Filters.Count > 0)
        {
            var expression = ExpressionMaker(
                ["id", "title", "genre", "author", "price", "inventoryStatus"],
                ["inventoryStatus"],
                dto.Filters
            );

            filteredData = queryable.Where(expression);
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
