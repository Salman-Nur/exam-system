using System.ComponentModel.DataAnnotations;

namespace ExamSystem.Application.Common.Options;

public record SmtpOptions
{
    public const string SectionName = "SmtpOptions";
    [Required] public required string SenderName { get; init; }
    [Required] public required string SenderEmail { get; init; }
    [Required] public required string Host { get; init; }
    [Required] public required string Username { get; init; }
    [Required] public required string Password { get; init; }
    [Required] public required int Port { get; init; }
    [Required] public required bool UseTls { get; init; }
}
