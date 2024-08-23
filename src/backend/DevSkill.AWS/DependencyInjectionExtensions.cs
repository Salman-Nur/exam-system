using Amazon.Runtime;
using Amazon.S3;
using DevSkill.AWS.Services;
using DevSkill.AWS.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DevSkill.AWS;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection RegisterDevSkillAmazonWebService(this IServiceCollection services,
        IConfiguration configuration, string accessKey, string secretKey)
    {
        var awsOpts = configuration.GetAWSOptions();

        ArgumentNullException.ThrowIfNull(awsOpts);
        ArgumentNullException.ThrowIfNull(accessKey);
        ArgumentNullException.ThrowIfNull(secretKey);

        awsOpts.Credentials = new BasicAWSCredentials(accessKey, secretKey);
        services.AddDefaultAWSOptions(awsOpts);
        services.AddAWSService<IAmazonS3>();

        services.TryAddScoped<IS3BucketService, S3BucketService>();
        return services;
    }
}
