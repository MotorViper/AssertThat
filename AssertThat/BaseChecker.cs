using System.Globalization;

namespace AssertThatLibrary;

public abstract class BaseChecker
{
    protected static bool? SimpleCompare(AssertThatParameters parameters)
    {
        var actual = parameters.Actual;
        var expected = parameters.Expected;
        if (actual == null && expected == null)
            return true;
        if (actual == null || expected == null)
            return false;
        var actualType = parameters.ActualType;
        var expectedType = parameters.ExpectedType;
        if (actualType == expectedType && actual is IComparable comparable)
            return comparable.CompareTo(expected) == 0;
        var method = actualType.GetMethod("op_Inequality", [expectedType]);
        if (method != null)
            return !(bool)method.Invoke(null, [actual, expected])!;
        method = actualType.GetMethod("op_Equality", [expectedType]);
        if (method != null)
            return (bool)method.Invoke(null, [actual, expected])!;
        method = actualType.GetMethod("op_Inequality");
        if (method != null)
            try
            {
                return !(bool)method.Invoke(null, [actual, expected])!;
            }
            catch
            {
                // ignored
            }

        method = actualType.GetMethod("op_Equality");
        if (method != null)
            try
            {
                return (bool)method.Invoke(null, [actual, expected])!;
            }
            catch
            {
                // ignored
            }

        if (actual is IConvertible convertible)
            try
            {
                var converted = convertible.ToType(expectedType, CultureInfo.CurrentCulture);
                if (converted != null)
                    return converted.Equals(expected);
            }
            catch
            {
                // ignored
            }

        return null;
    }

    protected string CreateMessage(string? propertyName, object? actual, object? expected)
    {
        propertyName = propertyName == null ? string.Empty : propertyName + " = ";
        return $"Actual and expected values are different: {propertyName}[{actual}] != [{expected}]";
    }
}