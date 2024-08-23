namespace ExamSystem.Web.Models.Account
{
    public class UserInformationModel
    {
        public required string ProfilePicture { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required uint MobileNumber { get; set; }
        public required string Biography { get; set; }
        public required string Gender { get; set; }
        public required string CurrentStatus { get; set; }
        public required string EducationalOrganisation { get; set; }
        public required string TopicsInterested { get; set; }
        public required string LinkedinProfileLink { get; set; }
        public required int CountryCode { get; set; }
        public required string Country { get; set; }
        public required string ReferralCode { get; set; }
        public required bool TermsAndConditions { get; set; }
    }
}
