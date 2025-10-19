using AssertThatLibrary.SearchKeys;

namespace AssertThatLibrary;

public record AssertThatOptions
{
    public Checkers Checkers { get; init; } = new();
    public SearchKeyDictionary<bool> CheckReadOnly { get; } = new();
    public SearchKeyDictionary<bool> CompareOrder { get; } = new();
    public bool Exact { get; init; }
    public Direction ReportMissingProperties { get; init; } = Direction.Both;
    public Func<object, string> OrderComparisonFunction { get; init; } = x => x?.ToString() ?? string.Empty;
}