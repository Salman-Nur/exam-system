using System.Net;
using DevSkill.AWS.Services.Enums;
using SharpOutcome;
using SharpOutcome.Helpers;

namespace DevSkill.AWS.Services.Contracts;

public interface IS3BucketService
{
    Task<ValueOutcome<HttpStatusCode, S3BadOutcomeTag>> StoreFileAsync(string bucketName,
        string fileNameWithExtension, Stream readStream, string contentType, CancellationToken ct);

    Task<ValueOutcome<string, S3BadOutcomeTag>> GetFileUrlAsync(string bucketName, string key, DateTime ttl);

    Task<ValueOutcome<HttpStatusCode, S3BadOutcomeTag>> DeleteFileAsync(string bucketName,
        string fileNameWithExtension, CancellationToken ct);

    Task<ValueOutcome<Successful, S3BadOutcomeTag>> IsBucketExistsAsync(string bucketName);
    Task<ValueOutcome<HttpStatusCode, S3BadOutcomeTag>> CreateBucketAsync(string bucketName);
}
