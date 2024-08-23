namespace ExamSystem.Domain.Entities.Shared;

public static class DomainEntityConstants
{
    public const string MemberEntityDbTableName = "Members";
    public const int MemberFullNameMaxLength = 100;
    public const int MemberBioMaxLength = 1000;
    public const int MemberEmailMaxLength = 256;
    public const int MemberDialCodeMaxLength = 10;
    public const int MemberCityMaxLength = 500;
    public const int MemberPostalCodeMaxLength = 25;
    public const int MemberAddressMaxLength = 1000;
    public const int MemberDevSkillUsernameMaxLength = 100;
    public const int MemberProfilePictureUriMaxLength = 2048;

    public const string LogEntityDbTableName = "Logs";
    public const int LogLevelMaxLength = 128;
}
