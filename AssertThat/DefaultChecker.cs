namespace AssertThatLibrary;

internal class DefaultChecker : BasePropertyChecker
{
    protected override string? CheckWhenNotNull(AssertThatNonNullParameters parameters)
    {
        var expectedProperty = parameters.ExpectedType.GetProperty(parameters.PropertyName!);
        FoundMatchableProperty = expectedProperty != null;
        if (!FoundMatchableProperty)
            return null;

        var expectedValue = expectedProperty!.GetValue(parameters.Expected, null);
        var newParameters = new AssertThatParameters
        {
            Actual = parameters.Actual,
            Expected = expectedValue,
            Options = parameters.Options,
            PropertyName = parameters.FullPropertyName
        };
        BaseValidator checker = parameters.ActualType == parameters.ExpectedType
            ? new IsValidator()
            : new IsLikeValidator();
        return checker.Check(newParameters);
    }
}