namespace AssertThatLibrary;

internal class EquivalentPropertyChecker(string equivalentPropertyName) : DefaultChecker
{
    public override string? Check(AssertThatParameters parameters) =>
        base.Check(parameters with { PropertyName = equivalentPropertyName });
}