using ExamSystem.Application.Common.Providers;

namespace ExamSystem.Infrastructure.Providers;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime CurrentUtcTime => TimeProvider.System.GetUtcNow().UtcDateTime;
    public DateTime CurrentLocalTime => TimeProvider.System.GetLocalNow().LocalDateTime;
}
