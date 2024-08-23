using StringMate.Enums;

namespace ExamSystem.Infrastructure;

public static class InfrastructureConstants
{
    public const RDBMS DatabaseInUse = RDBMS.SqlServer;
    public const string AccessTokenCookieKey = "ACCESS_TOKEN";
    public const string XsrfTokenCookieKey = "XSRF-TOKEN";
    public const string XsrfTokenHeaderKey = "X-XSRF-TOKEN";
}
