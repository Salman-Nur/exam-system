using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ExamSystem.Application.QuestionManagementFeatures.DataTransferObjects;
using ExamSystem.Application.QuestionManagementFeatures.Services;
using ExamSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ExamSystem.HttpApi.RequestHandlers;

public class QuestionCreateRequestHandler
{
    private string Title { get; set; }
    private IDictionary<int, string> DescriptionTextElements { get; set; } = new Dictionary<int, string>();
    private IDictionary<int, IFormFile> DescriptionImageElements { get; set; } = new Dictionary<int, IFormFile>();
    private IList<(int, bool, IFormFile)> OptionsImageElements { get; set; } = new List<(int, bool, IFormFile)>();
    private IList<(int, bool, string)> OptionTextElements { get; set; } = new List<(int, bool, string)>();
    private string[] Tags { get; set; }
    private byte Mark { get; set; }
    private ushort TimeLimit { get; set; }
    private bool Required { get; set; }
    private DifficultyLevel DifficultyLevel { get; set; }

    public async Task CreateQuestionAsync(IServiceProvider serviceProvider, IFormCollection collection, CancellationToken ct)
    {

        Title = collection[KeyName.Title].ToString();
        Mark = Convert.ToByte(collection[KeyName.Mark]);
        TimeLimit = Convert.ToUInt16(collection[KeyName.TimeLimit].ToString());
        Required = Convert.ToBoolean(collection[KeyName.Required]);
        DifficultyLevel = Enum.Parse<DifficultyLevel>(collection[KeyName.DifficultyLevel].ToString());
        Tags = collection[KeyName.Tags].ToString().Split(',').ToArray();

        await ProcessFiles(collection, DescriptionImageElements, OptionsImageElements);
        await ProcessFormFields(collection, DescriptionTextElements, OptionTextElements);


        var dto = new CreateQuestionDto(
            Title, Mark, TimeLimit, Tags, Required, DifficultyLevel,
            (DescriptionImageElements, DescriptionTextElements),
            (OptionsImageElements, OptionTextElements));

         var questionManagementService = serviceProvider.GetRequiredService<IQuestionManagementService>();
        await questionManagementService.CreateQuestionAsync(dto, ct);
    }

    private Task ProcessFormFields(IFormCollection formCollection, IDictionary<int, string> descriptions, IList<(int, bool, string)> options)
    {
        foreach (var key in formCollection.Keys)
        {
            if (key.StartsWith(FileName.Description.ToString()))
            {
                var serial = GetSerialNumber(key);
                descriptions.Add(serial, formCollection[key].ToString());
            }
            else if(key.StartsWith(FileName.Option.ToString()))
            {
                var serial = GetSerialNumber(key);
                var value = formCollection[key];
                if (value.Count == 1)
                {
                    var option = OptionsImageElements.FirstOrDefault(x => x.Item1 == serial);
                    option.Item2 = Convert.ToBoolean(value);
                }
                else
                {
                    options.Add((serial, Convert.ToBoolean(value[1]), value[0]!.ToString()));
                }
            }

        }
        return Task.CompletedTask;
    }
    private static Task ProcessFiles(IFormCollection formCollection, IDictionary<int, IFormFile> description, IList<(int, bool, IFormFile)> options)
    {
        foreach (var file in formCollection.Files)
        {
            if (file.Name.StartsWith(FileName.Description.ToString()))
            {
                var serial = GetSerialNumber(file.Name);
                description.Add(serial, file);
            }
            else if(file.Name.StartsWith(FileName.Option.ToString()))
            {
                var serial = GetSerialNumber(file.Name);
                options.Add((serial, false, file));
            }
        }
        return Task.CompletedTask;
    }
    private static int GetSerialNumber(string name)
    {
        return Convert.ToInt32(name.Split('-')[1]);
    }
}

internal enum FileName
{
    Description,
    Option
}

public static class KeyName
{
    public const string Title = "title";
    public const string Mark = "mark";
    public const string TimeLimit = "timeLimit";
    public const string Required = "required";
    public const string DifficultyLevel = "difficulty";
    public const string Tags = "tags";

}
