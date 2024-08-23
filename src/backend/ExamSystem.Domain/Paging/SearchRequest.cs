namespace ExamSystem.Domain.Paging;

public sealed class SearchRequest
{
    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public List<SortOrder> Sorts { get; set; } = [];

    public List<FilterColumn> Filters { get; set; } = [];
}
