using ExamSystem.Application.Common.Providers;

namespace ExamSystem.Infrastructure.Providers;

public class GuidProvider : IGuidProvider
{
    public Guid SortableGuid() => Ulid.NewUlid().ToGuid();

    public Guid RandomGuid() => Guid.NewGuid();
}
