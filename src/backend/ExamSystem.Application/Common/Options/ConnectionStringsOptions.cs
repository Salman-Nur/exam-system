using System.ComponentModel.DataAnnotations;

namespace ExamSystem.Application.Common.Options;

public record ConnectionStringsOptions
{
    public const string SectionName = "ConnectionStrings";
    [Required] public required string ExamSystemDb { get; init; }
}
