using ExamSystem.Domain.Enums;

namespace ExamSystem.Domain.Paging;

public class SortOrder
{
    public string SortBy { get; set; } = default!;
    public SortOrderType Order { get; set; }
}
