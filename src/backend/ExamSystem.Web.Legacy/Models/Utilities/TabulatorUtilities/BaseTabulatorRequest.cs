using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ExamSystem.Domain.Enums;
using ExamSystem.Domain.Paging;

namespace ExamSystem.Web.Models.Utilities.TabulatorUtilities;

public abstract class BaseTabulatorRequest
{
    [Required][JsonPropertyName("page")] public int Page { get; set; } = 1;
    [Required][JsonPropertyName("size")] public int Size { get; set; } = 10;

    [JsonPropertyName("filter")]
    public List<TabulatorSearch> Search { get; set; } = [];

    [JsonPropertyName("sort")]
    public List<TabulatorOrder> Order { get; set; } = [];

    public virtual SearchRequest GetSearchRequest()
    {
        var request = new SearchRequest
        {
            PageIndex = Page > 0 ? Page / Size + 1 : 1,
            PageSize = Size == 0 ? 10 : Size,
            Sorts = PrepareOrdering(),
            Filters = PrepareFiltering()
        };

        return request;
    }

    public virtual List<SortOrder> PrepareOrdering()
    {
        return Order.Select(order => new SortOrder
        {
            SortBy = order.Field,
            Order = order.Dir.ToLower() switch
            {
                "asc" => SortOrderType.Ascending,
                "desc" => SortOrderType.Descending,
                _ => SortOrderType.Ascending
            },
        }).ToList();
    }

    public abstract List<FilterColumn> PrepareFiltering();
}
