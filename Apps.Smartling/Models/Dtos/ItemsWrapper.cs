namespace Apps.Smartling.Models.Dtos;

public record ItemsWrapper<T>(IEnumerable<T> Items, int? TotalCount, object? Offset);