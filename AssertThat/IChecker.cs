namespace AssertThatLibrary;

public interface IChecker : IValidator
{
    IEnumerable<string> CheckedProperties { get; }
    bool FoundMatchableProperty { get; }
}