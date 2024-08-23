namespace ExamSystem.Application.Common.Services;

public interface IEmailService
{
    Task SendSingleEmailAsync(string receiverName, string receiverEmail, string subject, string body);
}
