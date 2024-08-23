using System.Globalization;
using ExamSystem.Domain.Paging;
using ExamSystem.Infrastructure.Extensions;
using Humanizer;

namespace ExamSystem.Web.Models.Utilities.TabulatorUtilities;

public class TabulatorRequest : BaseTabulatorRequest
{
    private const string InputDateFormat = "dd/MM/yyyy hh:mm tt";
    private const string OutputDateFormat = "MM/dd/yyyy hh:mm tt";

    public override List<FilterColumn> PrepareFiltering()
    {
        var filters = new List<FilterColumn>();

        Search.Where(x => !string.IsNullOrWhiteSpace(x.Value) && !string.IsNullOrWhiteSpace(x.Field)).ToList().ForEach(search =>
        {
            if (DateTime.TryParseExact(search.Value, InputDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var date))
            {
                filters.Add(new FilterColumn
                {
                    FilterBy = search.Field.Pascalize(),
                    Operator = QueryableExtensions.GetOperatorType(search.Type),
                    Value = date.ToString(OutputDateFormat)
                });
            }
            else
            {
                filters.Add(new FilterColumn
                {
                    FilterBy = search.Field.Pascalize(),
                    Operator = QueryableExtensions.GetOperatorType(search.Type),
                    Value = search.Value
                });
            }
        });

        return filters;
    }
}
