namespace ExamSystem.Application.MembershipFeatures.DataTransferObjects
{
    public record MemberListDTO
    {
        public required Guid Id { get; init; }
        public required string FullName { get; init; }
        public required string Email { get; init; }
        public required string UserName { get; init; }
        public required string ProfilePictureUri { get; init; }
    }
}
