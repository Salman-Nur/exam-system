using ExamSystem.Domain.Entities.McqAggregate;
using ExamSystem.Domain.Enums;
using ExamSystem.Domain.Misc;
using Microsoft.AspNetCore.Http;

namespace ExamSystem.Application.QuestionManagementFeatures.DataTransferObjects;

public record CreateQuestionDto(string Title, byte Mark, ushort TimeLimit, string[] Tags,
    bool Required, DifficultyLevel DifficultyLevel,
    (IDictionary<int, IFormFile> images, IDictionary<int, string> texts)  Body,
    (IList<(int, bool, IFormFile)> images, IList<(int, bool, string)> texts) Options);
