using System.ComponentModel.DataAnnotations;

namespace ExamSystem.Web.Models;

public class MarkdownEditorViewModel
{
    [Required][Display(Name = "Body")] public required string Body { get; set; }
}
