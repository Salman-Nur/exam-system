using ExamSystem.Domain.Enums;

namespace ExamSystem.Domain.Paging;

public sealed class FilterColumn
{
    public string FilterBy { get; set; } = default!;

    public OperatorType Operator { get; set; }

    public string Value { get; set; } = default!;
}
