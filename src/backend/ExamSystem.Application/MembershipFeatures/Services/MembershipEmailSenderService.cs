using ExamSystem.Application.Common.Services;

namespace ExamSystem.Application.MembershipFeatures.Services;

public class MembershipEmailSenderService : IMembershipEmailSenderService
{
    private readonly IEmailService _emailService;

    public MembershipEmailSenderService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendVerificationMailAsync(string fullName, string email, string verificationCode)
    {
        const string subject = "Account Confirmation";

        var body = $"""
                    <html>
                        <body>
                            <h2>Welcome, {fullName}!</h2>
                            <p>Thanks for signing up. Please verify your email address by using the following verification code:</p>
                            <h2>{verificationCode}</h2>
                            <p>If you didn't request this, you can safely ignore this email.</p>
                            <p>Have a good day.</p>
                        </body>
                    </html>
                    """;

        await _emailService.SendSingleEmailAsync(fullName, email, subject, body);
    }

    public async Task SendResetPasswordMailAsync(string fullName, string email, string code)
    {
        const string subject = "Reset Password";

        var body = $"""
                    <html>
                        <body>
                            <h2>Hello, {fullName}!</h2>
                            <p>Please change your password by using the following code:</p>
                            <h2>{code}</h2>
                            <p>If you didn't request this, you can safely ignore this email.</p>
                            <p>Have a good day.</p>
                        </body>
                    </html>
                    """;

        await _emailService.SendSingleEmailAsync(fullName, email, subject, body);
    }
}
