using System.Text.Json;
using System.Text.Json.Serialization;
using DotNetEnv;
using ExamSystem.Application;
using ExamSystem.Application.Common.Options;
using ExamSystem.HttpApi;
using ExamSystem.HttpApi.Others;
using ExamSystem.Infrastructure;
using ExamSystem.Infrastructure.Extensions;
using Microsoft.AspNetCore.Antiforgery;
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

    builder.Services.BindAndValidateOptions<ConnectionStringsOptions>(ConnectionStringsOptions.SectionName);
    builder.Services.BindAndValidateOptions<SerilogEmailSinkOptions>(SerilogEmailSinkOptions.SectionName);
    builder.Services.BindAndValidateOptions<AwsCredentialOptions>(AwsCredentialOptions.SectionName);
    builder.Services.BindAndValidateOptions<SmtpOptions>(SmtpOptions.SectionName);
    builder.Services.BindAndValidateOptions<JwtOptions>(JwtOptions.SectionName);
    builder.Services.BindAndValidateOptions<AppOptions>(AppOptions.SectionName);

    builder.Services.AddSerilog((_, lc) => lc
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(configuration)
        .WriteTo.ConfigureEmailSink(builder.Configuration)
    );

    builder.Services.AddControllers(opts =>
            {
                opts.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                opts.OutputFormatters.RemoveType<StringOutputFormatter>();
                opts.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
            }
        )
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            opts.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = ctx => ctx.MakeValidationErrorResponse();
        });

    var appOptions = configuration.GetRequiredSection(AppOptions.SectionName).Get<AppOptions>();

    ArgumentNullException.ThrowIfNull(appOptions);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(nameof(AppOptions.AllowedOriginsForCors), x => x
            .WithOrigins(appOptions.AllowedOriginsForCors)
            .WithExposedHeaders(InfrastructureConstants.XsrfTokenHeaderKey)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
        );
    });

    builder.Services.AddAntiforgery(options =>
    {
        options.Cookie.Name = InfrastructureConstants.XsrfTokenCookieKey;
        options.HeaderName = InfrastructureConstants.XsrfTokenHeaderKey;
    });

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    var recaptchaOptions = configuration.GetRequiredSection(GoogleRecaptchaOptions.SectionName)
        .Get<GoogleRecaptchaOptions>();

    ArgumentNullException.ThrowIfNull(recaptchaOptions);

    builder.Services.AddHttpClient(GoogleRecaptchaOptions.SectionName,
        httpClient => httpClient.BaseAddress = new Uri(recaptchaOptions.VerificationEndpoint));

    builder.Services.AddSwaggerGen();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.RegisterHttpApiServices();
    builder.Services.RegisterInfrastructureServices();
    builder.Services.RegisterApplicationServices();

    ApplicationMappingConfig.RegisterMappings();

    builder.Services.AddAuthorizationPolicies(builder.Configuration);
    builder.Services.AddIdentityConfiguration(builder.Configuration);
    builder.Services.AddDatabaseConfig(builder.Configuration);
    builder.Services.AddAwsConfig(builder.Configuration);
    builder.Services.AddJwtAuth(builder.Configuration);
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddCustomKestrelAndDataProtection(builder.Configuration, appOptions);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors(nameof(AppOptions.AllowedOriginsForCors));
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseExceptionHandler();
    app.MapControllers();

    app.MapPost("api/v1/load-xsrf-token", (HttpContext ctx, IAntiforgery antiForgery) =>
    {
        var token = antiForgery.GetAndStoreTokens(ctx);
        ctx.Response.Headers.Append(InfrastructureConstants.XsrfTokenHeaderKey, token.RequestToken);
    }).WithTags("Security");

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
