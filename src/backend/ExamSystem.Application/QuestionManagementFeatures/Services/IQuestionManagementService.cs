using ExamSystem.Application.QuestionManagementFeatures.DataTransferObjects;

namespace ExamSystem.Application.QuestionManagementFeatures.Services;

public interface IQuestionManagementService
{
    Task CreateQuestionAsync(CreateQuestionDto createQuestionDto, CancellationToken ct);
}
