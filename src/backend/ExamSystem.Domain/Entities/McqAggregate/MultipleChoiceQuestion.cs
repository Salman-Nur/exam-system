using ExamSystem.Domain.Entities.Shared.Abstracts;
using ExamSystem.Domain.Enums;
using ExamSystem.Domain.Misc;

namespace ExamSystem.Domain.Entities.McqAggregate;

public class MultipleChoiceQuestion : Question
{
    public bool IsPartialMarkingAllowed { get; set; }
    public HashSet<MultipleChoiceQuestionOption> Options { get; set; }
    public HashSet<MultipleChoiceQuestionTag> Tags { get; set; }

    public MultipleChoiceQuestion(string title, byte score, bool isRequired,
        ushort timeLimit, DifficultyLevel difficultyLevel)
    {
        Title = title;
        Score = score;
        IsRequired = isRequired;
        TimeLimit = timeLimit;
        DifficultyLevel = difficultyLevel;
        CreatedAtUtc = DateTime.Now;
        Options = new HashSet<MultipleChoiceQuestionOption>();
        Tags = new HashSet<MultipleChoiceQuestionTag>();
    }

    public void AddOption(MultipleChoiceQuestionOption opt)
        => Options.Add(opt);

    public void RemoveOption(MultipleChoiceQuestionOption opt)
        => Options.Remove(opt);

    public void AddTag(MultipleChoiceQuestionTag tag)
        => Tags.Add(tag);

    public void RemoveTag(MultipleChoiceQuestionTag tag)
        => Tags.Remove(tag);

    public void AddContent(Content content)
    {
        Body = content;
    }

    public void AddOptionContent(HashSet<MultipleChoiceQuestionOption> options)
    {
        Options = options;
    }

    public void AddTags(HashSet<MultipleChoiceQuestionTag> tags)
    {
        Tags = tags;
    }

}
