namespace AssertThatLibrary;

internal class IgnoredItemChecker : IChecker
{
    public IEnumerable<string> CheckedProperties { get; private set; } = Enumerable.Empty<string>();

    public bool FoundMatchableProperty => true;

    public string? Check(AssertThatParameters parameters)
    {
        CheckedProperties = [parameters.PropertyName!];
        return null;
    }
}