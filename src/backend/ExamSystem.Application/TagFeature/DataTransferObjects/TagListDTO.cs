namespace ExamSystem.Application.TagFeature.DataTransferObjects
{
    public record TagListDTO
    {
        public required Guid Id { get; init; }
        public required string Name { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
