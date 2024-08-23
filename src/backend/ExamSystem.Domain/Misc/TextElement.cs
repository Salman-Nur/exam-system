using ExamSystem.Domain.Entities.Shared.Abstracts;

namespace ExamSystem.Domain.Misc;

public class TextElement : ContentElement
{
    public required string Body { get; set; }
}
