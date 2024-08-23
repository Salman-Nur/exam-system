using System.Net;
using ExamSystem.Application.Common.Options;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.Email;

namespace ExamSystem.Infrastructure.Extensions;

public static class SerilogExtensions
{
    public static LoggerConfiguration ConfigureEmailSink(this LoggerSinkConfiguration instance,
        IConfiguration configuration)
    {
        var serilogEmailSinkOptions = configuration
            .GetRequiredSection(SerilogEmailSinkOptions.SectionName)
            .Get<SerilogEmailSinkOptions>();

        ArgumentNullException.ThrowIfNull(serilogEmailSinkOptions, nameof(SerilogEmailSinkOptions));

        return instance.Email(
            options: new EmailSinkOptions
            {
                From = serilogEmailSinkOptions.EmailFrom,
                To = [serilogEmailSinkOptions.EmailTo],
                Subject = new MessageTemplateTextFormatter(serilogEmailSinkOptions.EmailSubject),
                Credentials = new NetworkCredential
                {
                    UserName = serilogEmailSinkOptions.SmtpUsername,
                    Password = serilogEmailSinkOptions.SmtpPassword
                },
                Host = serilogEmailSinkOptions.SmtpHost,
                Port = serilogEmailSinkOptions.SmtpPort
            },
            restrictedToMinimumLevel: Enum.Parse<LogEventLevel>(serilogEmailSinkOptions.MinimumLogLevel)
        );
    }
}
