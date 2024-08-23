using ExamSystem.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace ExamSystem.Application.MembershipFeatures.DataTransferObjects
{
    public class MemberUpdateDTO
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public string? Email { get; set; }
    }
}
