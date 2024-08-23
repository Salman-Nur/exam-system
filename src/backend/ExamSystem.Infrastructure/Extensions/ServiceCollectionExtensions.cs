using System.Text;
using DevSkill.AWS;
using ExamSystem.Application.Common.Options;
using ExamSystem.Infrastructure.Identity.Constants;
using ExamSystem.Infrastructure.Identity.Managers;
using ExamSystem.Infrastructure.Identity.Persistence;
using ExamSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ExamSystem.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static bool IsRunningInContainer(this IConfiguration configuration) =>
        configuration.GetValue<bool>("DOTNET_RUNNING_IN_CONTAINER");

    public static IServiceCollection BindAndValidateOptions<TOptions>(this IServiceCollection services,
        string sectionName) where TOptions : class
    {
        services.AddOptions<TOptions>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return services;
    }

    public static IServiceCollection AddAwsConfig(this IServiceCollection services,
        IConfiguration configuration)
    {
        var awsCredentialOptions = configuration
            .GetRequiredSection(AwsCredentialOptions.SectionName)
            .Get<AwsCredentialOptions>();

        ArgumentNullException.ThrowIfNull(awsCredentialOptions);

        services.RegisterDevSkillAmazonWebService(configuration, awsCredentialOptions.AccessKey,
            awsCredentialOptions.SecretKey);

        return services;
    }

    public static IServiceCollection AddDatabaseConfig(this IServiceCollection services,
        IConfiguration configuration)
    {
        var dbUrl = configuration.GetRequiredSection(ConnectionStringsOptions.SectionName)
            .GetValue<string>(nameof(ConnectionStringsOptions.ExamSystemDb));

        services.AddDbContext<ExamSystemDbContext>(
            dbContextOptions => dbContextOptions
                .UseSqlServer(dbUrl)
                .UseEnumCheckConstraints()
        );

        return services;
    }

    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        var dbUrl = configuration.GetRequiredSection(ConnectionStringsOptions.SectionName)
            .GetValue<string>(nameof(ConnectionStringsOptions.ExamSystemDb));

        services.AddDbContext<ApplicationIdentityDbContext>(
            dbContextOptions => dbContextOptions
                .UseSqlServer(dbUrl)
                .UseEnumCheckConstraints()
        );

        const string allowedCharsInPassword =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

        services.AddIdentity<ApplicationIdentityUser, ApplicationRole>(
                options =>
                {
                    // for 6 digit numeric code
                    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                }
            )
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
            .AddUserManager<ApplicationUserManager>()
            .AddRoleManager<ApplicationRoleManager>()
            .AddSignInManager<ApplicationSignInManager>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            options.User.AllowedUserNameCharacters = allowedCharsInPassword;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedAccount = true;
        });

        return services;
    }

    public static IServiceCollection AddCookieAuthentication(this IServiceCollection services)
    {
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = new PathString("/account/login");
            options.AccessDeniedPath = new PathString("/home/error/401");
            options.LogoutPath = new PathString("/account/logout");
            options.Cookie.Name = "Identity";
            options.SlidingExpiration = true;
        });

        return services;
    }

    public static IServiceCollection AddJwtAuth(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        ArgumentNullException.ThrowIfNull(jwtOptions);


        services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[InfrastructureConstants.AccessTokenCookieKey];
                        return Task.CompletedTask;
                    }
                };
            });
        return services;
    }

    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(PolicyConstants.MemberPolicy, policy => { policy.RequireAuthenticatedUser(); });

        services.AddAuthorizationBuilder()
            .AddPolicy(PolicyConstants.InternalUserPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypeConstants.InternalUser, bool.TrueString);
            })
            .AddPolicy(PolicyConstants.ViewDashboardPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypeConstants.ViewDashboard, bool.TrueString);
            })
            .AddPolicy(PolicyConstants.ManageMemberClaimPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypeConstants.ManageMemberClaim, bool.TrueString);
            })
            .AddPolicy(PolicyConstants.ManageMemberPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypeConstants.ManageMember, bool.TrueString);
            })
            .AddPolicy(PolicyConstants.ManageQuestionPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypeConstants.ManageQuestion, bool.TrueString);
            })
            .AddPolicy(PolicyConstants.QuestionCreatePolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypeConstants.QuestionCreate, bool.TrueString);
            })
            .AddPolicy(PolicyConstants.QuestionViewPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypeConstants.QuestionView, bool.TrueString);
            })
            .AddPolicy(PolicyConstants.QuestionEditPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypeConstants.QuestionEdit, bool.TrueString);
            })
            .AddPolicy(PolicyConstants.QuestionDeletePolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypeConstants.QuestionDelete, bool.TrueString);
            })
            .AddPolicy(PolicyConstants.ManageExamPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypeConstants.ManageExam, bool.TrueString);
            })
            .AddPolicy(PolicyConstants.ExamCreatePolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypeConstants.ExamCreate, bool.TrueString);
            })
            .AddPolicy(PolicyConstants.ExamViewPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypeConstants.ExamView, bool.TrueString);
            })
            .AddPolicy(PolicyConstants.ExamEditPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypeConstants.ExamEdit, bool.TrueString);
            })
            .AddPolicy(PolicyConstants.ExamDeletePolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypeConstants.ExamDelete, bool.TrueString);
            })
            .AddPolicy(PolicyConstants.ManageLogPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypeConstants.ManageLog, bool.TrueString);
            });

        return services;
    }

    public static IServiceCollection AddCustomKestrelAndDataProtection(this IServiceCollection services,IConfiguration configuration,AppOptions appOptions)
    {
        string fullPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", appOptions.DataProtectionDirectoryPath));

        services.Configure<KestrelServerOptions>(configuration.GetSection("Kestrel"));

        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(fullPath));

        return services;
    }
}
