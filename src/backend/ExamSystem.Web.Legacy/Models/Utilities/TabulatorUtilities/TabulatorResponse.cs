using System.Text.Json.Serialization;
using ExamSystem.Domain.Paging;

namespace ExamSystem.Web.Models.Utilities.TabulatorUtilities;

public sealed class TabulatorResponse<T>(int recordsTotal, int pageSize, IReadOnlyList<T> data)
    where T : class
{
    [JsonPropertyName("last_row")]
    public int RecordsTotal { get; } = data.Count;

    [JsonPropertyName("last_page")]
    public int RecordsFiltered { get; } = (int)Math.Ceiling(recordsTotal / (decimal)pageSize);

    [JsonPropertyName("data")]
    public IReadOnlyList<T> Data { get; } = data;

    public static TabulatorResponse<T> ToResponse(int pageSize, IPaginate<T> pagingData)
        => new(pagingData.Total, pageSize, pagingData.Items.ToList());

    public static TabulatorResponse<T> ToResponse(int totalData, int pageSize, IReadOnlyList<T> data)
        => new(totalData, pageSize, data);

    public static TabulatorResponse<T> Empty() => new(0, 0, []);
}
