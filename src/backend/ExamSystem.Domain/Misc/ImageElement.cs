using ExamSystem.Domain.Entities.Shared.Abstracts;

namespace ExamSystem.Domain.Misc;

public class ImageElement : ContentElement
{
    public required string Uri { get; set; }
}
