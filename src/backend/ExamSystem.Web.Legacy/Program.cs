using System.Text.Json;
using System.Text.Json.Serialization;
using AspNetCore.ReCaptcha;
using DotNetEnv;
using ExamSystem.Application;
using ExamSystem.Application.Common.Options;
using ExamSystem.Infrastructure;
using ExamSystem.Infrastructure.Extensions;
using ExamSystem.Web;
using ExamSystem.Web.Others;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Serilog;
using Serilog.Events;

Env.Load();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration
        .AddJsonFile("appsettings.json", optional: false)
        .AddEnvironmentVariables();

    builder.Services.BindAndValidateOptions<GoogleRecaptchaOptions>(GoogleRecaptchaOptions.SectionName);
    builder.Services.BindAndValidateOptions<ConnectionStringsOptions>(ConnectionStringsOptions.SectionName);
    builder.Services.BindAndValidateOptions<SerilogEmailSinkOptions>(SerilogEmailSinkOptions.SectionName);
    builder.Services.BindAndValidateOptions<AwsCredentialOptions>(AwsCredentialOptions.SectionName);
    builder.Services.BindAndValidateOptions<SmtpOptions>(SmtpOptions.SectionName);
    builder.Services.BindAndValidateOptions<AppOptions>(AppOptions.SectionName);

    builder.Services.AddSerilog((_, lc) => lc
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(configuration)
        .WriteTo.ConfigureEmailSink(builder.Configuration)
    );

    var recaptchaOpts = builder.Configuration.GetRequiredSection(GoogleRecaptchaOptions.SectionName);
    builder.Services.AddReCaptcha(recaptchaOpts);

    builder.Services.Configure<RouteOptions>(options =>
    {
        options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
        options.AppendTrailingSlash = false;
    });

    builder.Services.AddControllersWithViews(opts =>
            {
                opts.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                opts.OutputFormatters.RemoveType<StringOutputFormatter>();
                opts.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
            }
        )
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
        });

    builder.Services.RegisterWebServices();
    builder.Services.RegisterInfrastructureServices();
    builder.Services.RegisterApplicationServices();

    builder.Services.AddAuthorizationPolicies(builder.Configuration);
    builder.Services.AddIdentityConfiguration(builder.Configuration);
    builder.Services.AddDatabaseConfig(builder.Configuration);
    builder.Services.AddAwsConfig(builder.Configuration);
    builder.Services.AddCookieAuthentication();
    builder.Services.AddHttpContextAccessor();

    var app = builder.Build();

    app.UseStatusCodePagesWithReExecute("/home/error/{0}");

    if (app.Environment.IsDevelopment() is false)
    {
        app.UseExceptionHandler("/home/error");
        app.UseHsts();
    }

    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists:slugify}/{controller:slugify=Home}/{action:slugify=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller:slugify=Home}/{action:slugify=Index}/{id?}");

    await app.RunAsync();
    return 0;
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application start-up failed");
    return 1;
}
finally
{
    Log.Information("Shut down complete");
    await Log.CloseAndFlushAsync();
}
