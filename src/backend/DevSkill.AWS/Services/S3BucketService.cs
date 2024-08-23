using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using DevSkill.AWS.Services.Contracts;
using DevSkill.AWS.Services.Enums;
using SharpOutcome;
using SharpOutcome.Helpers;

namespace DevSkill.AWS.Services;

public class S3BucketService : IS3BucketService
{
    private readonly IAmazonS3 _s3Client;

    public S3BucketService(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task<ValueOutcome<HttpStatusCode, S3BadOutcomeTag>> StoreFileAsync(string bucketName,
        string fileNameWithExtension, Stream readStream, string contentType, CancellationToken ct)
    {
        var bucketExistsOutcome = await IsBucketExistsAsync(bucketName);

        if (bucketExistsOutcome.TryPickBadOutcome(out var bucketNotExistsError))
        {
            return bucketNotExistsError;
        }

        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = fileNameWithExtension,
            ContentType = contentType,
            InputStream = readStream
        };

        var result = await _s3Client.PutObjectAsync(putObjectRequest, ct);
        return result.HttpStatusCode;
    }

    public async Task<ValueOutcome<string, S3BadOutcomeTag>> GetFileUrlAsync(string bucketName, string key,
        DateTime ttl)
    {
        var request = new GetPreSignedUrlRequest { BucketName = bucketName, Key = key, Expires = ttl };
        return await _s3Client.GetPreSignedURLAsync(request);
    }

    public async Task<ValueOutcome<HttpStatusCode, S3BadOutcomeTag>> DeleteFileAsync(string bucketName,
        string fileNameWithExtension, CancellationToken ct)
    {
        var bucketExistsOutcome = await IsBucketExistsAsync(bucketName);

        if (bucketExistsOutcome.TryPickBadOutcome(out var bucketNotExistsError))
        {
            return bucketNotExistsError;
        }

        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = fileNameWithExtension
        };

        var response = await _s3Client.DeleteObjectAsync(deleteObjectRequest, ct);
        return response.HttpStatusCode;
    }

    public async Task<ValueOutcome<Successful, S3BadOutcomeTag>> IsBucketExistsAsync(string bucketName)
    {
        if (await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName) is not false)
        {
            return new Successful();
        }

        return S3BadOutcomeTag.BucketNotFound;
    }

    public async Task<ValueOutcome<HttpStatusCode, S3BadOutcomeTag>> CreateBucketAsync(string bucketName)
    {
        if (AmazonS3Util.ValidateV2Bucket(bucketName) is false)
        {
            return S3BadOutcomeTag.InvalidBucketName;
        }

        var putBucketRequest = new PutBucketRequest { BucketName = bucketName, UseClientRegion = true };
        var result = await _s3Client.PutBucketAsync(putBucketRequest);
        return result.HttpStatusCode;
    }
}
