using System.ComponentModel;

namespace AssertThatLibrary;

internal class AutoConvertChecker : BasePropertyChecker
{
    protected override string? CheckNotNull(AssertThatNonNullParameters parameters)
    {
        var expectedType = parameters.ExpectedType;
        var propertyName = parameters.PropertyName;
        var expectedProperty = expectedType.GetProperty(propertyName!);
        var fullPropertyName = parameters.FullPropertyName;
        FoundMatchableProperty = expectedProperty != null;
        if (!FoundMatchableProperty)
            return $"{fullPropertyName}: {propertyName} is not in {expectedType.Name}";

        var expectedValue = expectedProperty!.GetValue(parameters.Expected, null);
        var actual = parameters.Actual;
        var matches = CheckConverted(actual, expectedValue!, expectedType) ??
                      CheckConverted(expectedValue!, actual, parameters.ActualType);
        if (matches.HasValue)
            return matches.Value
                ? null
                : $"Actual and expected values are different: {fullPropertyName}[{actual}]/[{expectedValue}]";
        return $"Could not convert between {parameters.ActualType.Name} and {expectedType.Name}";
    }

    private bool? CheckConverted(object from, object to, Type toType)
    {
        var converter = TypeDescriptor.GetConverter(toType);
        object converted;
        if (converter.CanConvertFrom(from.GetType()))
        {
            converted = converter.ConvertFrom(from)!;
            return new IsValidator().Check(new AssertThatParameters(converted, to)) == null;
        }

        if (from is IConvertible)
        {
            converted = Convert.ChangeType(from, toType);
            return new IsValidator().Check(new AssertThatParameters(converted, to)) == null;
        }

        return null;
    }
}