using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ExamSystem.Web.Dto;

public class TabulatorQueryDto
{
    [Required][JsonPropertyName("size")] public int Size { get; set; }
    [Required][JsonPropertyName("page")] public int Page { get; set; } = 1;
    [JsonPropertyName("filter")] public List<TabulatorFilterDto> Filters { get; init; } = [];
    [JsonPropertyName("sort")] public IList<TabulatorSortingDto> Sorters { get; init; } = [];
}
