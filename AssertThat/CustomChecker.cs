namespace AssertThatLibrary;

internal class CustomChecker(Func<AssertThatParameters, string?> customChecker, Func<List<string>>? checkedNames = null)
    : IChecker
{
    public IEnumerable<string> CheckedProperties { get; private set; } = Enumerable.Empty<string>();

    public bool FoundMatchableProperty => true;

    public string? Check(AssertThatParameters parameters)
    {
        CheckedProperties = checkedNames == null ? [parameters.PropertyName!] : checkedNames();
        return customChecker(parameters);
    }
}