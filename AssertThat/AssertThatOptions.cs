namespace AssertThatLibrary;

public record AssertThatOptions
{
    public Checkers Checkers { get; init; } = new();
    public List<string> CheckReadOnly { get; init; } = ["*"];
    public bool? CompareOrder { get; init; } = true;
    public bool Exact { get; init; }
    public Direction ReportMissingProperties { get; init; } = Direction.Both;
    public Func<object, string> OrderComparisonFunction { get; init; } = x => x?.ToString() ?? string.Empty;

    public IValidator CreateValidator() => Exact ? new IsValidator() : new IsLikeValidator();
}