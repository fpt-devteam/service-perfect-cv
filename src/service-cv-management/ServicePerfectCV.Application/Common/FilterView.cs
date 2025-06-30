namespace ServicePerfectCV.Application.Common;

public class FilterView<T>
    where T : class
{
    public required int Count { get; init; }
    public required IEnumerable<T> Items { get; set; } = [];
}