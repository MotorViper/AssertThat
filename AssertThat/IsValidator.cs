using System.Reflection;

namespace AssertThatLibrary;

internal class IsValidator : BaseValidator
{
    protected override string? Check()
    {
        var matches = SimpleCompare(Parameters);
        if (matches.HasValue)
        {
            Parameters = Parameters with
            {
                Actual = Parameters.Actual ?? "null",
                Expected = Parameters.Expected ?? "null"
            };
            return matches.Value ? null : CreateMessage();
        }

        var properties = Parameters.ActualType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
            if (CheckProperty(property))
            {
                var actualValue = property.GetValue(Parameters.Actual, null);
                var expectedValue = property.GetValue(Parameters.Expected, null);
                var checker = new IsValidator();
                return checker.Check(new AssertThatParameters
                {
                    Actual = actualValue,
                    Expected = expectedValue,
                    Options = Parameters.Options,
                    PropertyName = FullPropertyName(property)
                });
            }

        return null;
    }
}