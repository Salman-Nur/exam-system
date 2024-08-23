using Amazon.S3;
using DevSkill.AWS.Services.Contracts;
using ExamSystem.Application.Common.Contracts;
using ExamSystem.Application.Common.Options;
using ExamSystem.Application.Common.Providers;
using ExamSystem.Application.QuestionManagementFeatures.DataTransferObjects;
using ExamSystem.Domain.Entities.McqAggregate;
using ExamSystem.Domain.Entities.Shared.Abstracts;
using ExamSystem.Domain.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ExamSystem.Application.QuestionManagementFeatures.Services;

public class QuestionManagementService(
    IApplicationUnitOfWork applicationUnitOfWork,
    IS3BucketService bucketService,
    IGuidProvider guidProvider,
    IOptions<AppOptions> appOptions) : IQuestionManagementService
{
    private readonly AppOptions _appOptions = appOptions.Value;
    public async Task CreateQuestionAsync(CreateQuestionDto createQuestionDto, CancellationToken ct)
    {
        var question = new MultipleChoiceQuestion(
            createQuestionDto.Title,
            createQuestionDto.Mark,
            createQuestionDto.Required,
            createQuestionDto.TimeLimit,
            createQuestionDto.DifficultyLevel)
        {
            Id = guidProvider.SortableGuid()
        };

        var bodyElements = new HashSet<ContentElement>();
        var bodyImages = createQuestionDto.Body.images;
        var bodyTexts = createQuestionDto.Body.texts;

        if (bodyTexts.Count > 0)
        {
            var elements = ConvertToContentElement(bodyTexts);
            foreach (var element in elements)
            {
                bodyElements.Add(element);
            }
        }

        if (bodyImages.Count > 0)
        {
            var elements = await AddToContentAsync(createQuestionDto.Body.images, ct);
            foreach (var contentElement in elements)
            {
                bodyElements.Add(contentElement);
            }
        }

        var content = new Content()
        {
            Id = guidProvider.SortableGuid(),
            Elements = bodyElements
        };

        var options = new HashSet<MultipleChoiceQuestionOption>();

        var optionImages = createQuestionDto.Options.images;
        var optionTexts = createQuestionDto.Options.texts;

        if (optionImages.Count > 0)
        {
            var dictionary = optionImages.ToDictionary(image => image.Item1, image => image.Item3);
            var elements = await AddToContentAsync(dictionary, ct);
            foreach (var option in from contentElement in elements let serial = contentElement.Serial select new MultipleChoiceQuestionOption()
                     {
                         Id = guidProvider.RandomGuid(),
                         MultipleChoiceQuestionId = question.Id,
                         IsCorrect = optionImages.Where(x => x.Item1 == serial).Select(x => x.Item2).FirstOrDefault(),
                         Body = contentElement,
                         BodyType = BodyType.Image
                     })
            {
                options.Add(option);
            }
        }

        if (optionTexts.Count > 0)
        {
            foreach (var (serial, isCorrect, text) in optionTexts)
            {
                var element = new TextElement()
                {
                    Id = guidProvider.RandomGuid(), Serial = Convert.ToUInt32(serial), Body = text
                };
                var option = new MultipleChoiceQuestionOption()
                {
                    Id = guidProvider.RandomGuid(),
                    MultipleChoiceQuestionId = question.Id,
                    IsCorrect = isCorrect,
                    Body = element,
                    BodyType = BodyType.Text
                };
                options.Add(option);
            }
        }

        question.AddContent(content);
         question.AddOptionContent(options);
         question.AddTags(ConvertToTags(question.Id, createQuestionDto.Tags));

         await applicationUnitOfWork
             .QuestionRepository
             .CreateAsync(question, ct)
             .ConfigureAwait(ConfigureAwaitOptions.None);

         await applicationUnitOfWork
             .SaveAsync()
             .ConfigureAwait(ConfigureAwaitOptions.None);
    }

    private HashSet<MultipleChoiceQuestionTag> ConvertToTags(Guid questionId, string[] tags)
    {
        var tagSet = new HashSet<MultipleChoiceQuestionTag>();
        foreach (var tagid in tags)
        {
            var tagEntity = new MultipleChoiceQuestionTag(Guid.Parse(tagid), questionId);
            tagSet.Add(tagEntity);
        }

        return tagSet;
    }

    private HashSet<ContentElement> ConvertToContentElement(IDictionary<int, string> texts)
    {
        var elements = new HashSet<ContentElement>();
        foreach (var (key, value) in texts)
        {
            var element = new TextElement()
            {
                Id = guidProvider.RandomGuid(), Serial = Convert.ToUInt32(key), Body = value
            };
            elements.Add(element);
        }

        return elements;
    }
    private async Task<HashSet<ContentElement>> AddToContentAsync<T>(T descriptions, CancellationToken ct)
            where T : IDictionary<int, IFormFile>
    {
        var ifExist = await bucketService.IsBucketExistsAsync(_appOptions.S3BucketName)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (ifExist.IsBadOutcome())
        {
            var isCreated = await bucketService.CreateBucketAsync(_appOptions.S3BucketName)
                .ConfigureAwait(ConfigureAwaitOptions.None);
            if (isCreated.IsBadOutcome())
            {
                throw new AmazonS3Exception("Failed to Create Bucket");
            }
        }

        var elements = new HashSet<ContentElement>();
        var uploadTasks = new List<Task>();

        foreach (var (key, value) in descriptions)
        {
            var fileName = guidProvider.RandomGuid() + value.FileName;
            uploadTasks.Add(UploadFileAsync(fileName, value, ct));

            var element = new ImageElement()
            {
                Id = guidProvider.RandomGuid(), Serial = Convert.ToUInt32(key), Uri = fileName
            };
            elements.Add(element);
        }

        await Task.WhenAll(uploadTasks);
        return elements;
    }

    private async Task UploadFileAsync(string filename, IFormFile file, CancellationToken ct)
    {
        var result = await bucketService.StoreFileAsync(
            _appOptions.S3BucketName,
            filename,
            file.OpenReadStream(),
            file.ContentType, ct).ConfigureAwait(ConfigureAwaitOptions.None);

        if (result.IsBadOutcome())
        {
            throw new AmazonS3Exception("Failed to Upload File");
        }
    }
}
