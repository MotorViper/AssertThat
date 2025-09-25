namespace AssertThatLibrary;

public record AssertThatParameters
{
    private Type? _actualType;
    private Type? _expectedType;
    private AssertThatOptions? _options;

    public AssertThatParameters()
    {
    }

    public AssertThatParameters(object? actual, object? expected, AssertThatOptions? options, StopWhen stopWhen)
    {
        Actual = actual;
        Expected = expected;
        _options = options;
        StopWhen = stopWhen;
    }

    public object? Actual { get; init; }

    public StopWhen StopWhen { get; init; } = StopWhen.Match;

    public Type ActualType
    {
        get { return _actualType ??= Actual!.GetType(); }
        init => _actualType = value;
    }

    public object? Expected { get; init; }

    public Type ExpectedType
    {
        get { return _expectedType ??= Expected!.GetType(); }
        init => _expectedType = value;
    }

    public AssertThatOptions Options
    {
        get { return _options ??= new AssertThatOptions(); }
        init => _options = value;
    }

    public string? PropertyName { get; init; }
    public string? FullPropertyName { get; init; }
}