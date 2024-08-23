using System.ComponentModel.DataAnnotations;

namespace ExamSystem.Application.Common.Options;

public record AppOptions
{
    public const string SectionName = "AppOptions";
    [Required] public required string[] AllowedOriginsForCors { get; init; }
    [Required] public required string S3BucketName { get; init; }
    [Required] public required string DataProtectionDirectoryPath { get; init; }
}
