using System.Reflection;

namespace AssertThatLibrary;

internal class IsLikeValidator : BaseValidator
{
    protected override string? Check()
    {
        var matches = SimpleCompare(Parameters) ??
                      SimpleCompare(Parameters with
                      {
                          Actual = Parameters.Expected,
                          ActualType = Parameters.ExpectedType,
                          Expected = Parameters.Actual,
                          ExpectedType = Parameters.ActualType
                      });
        if (matches.HasValue)
            return matches.Value ? null : CreateMessage();

        var actualType = Parameters.ActualType;
        var properties = actualType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var expectedType = Parameters.ExpectedType;
        var missingActualProperties = new List<string>();
        var checkedExpectedProperties = new List<string>();
        foreach (var property in GetPropertiesToCheck(properties))
        {
            var newFullPropertyName = FullPropertyName(property);
            var checker = Parameters.Options.Checkers.GetValueOrDefault(newFullPropertyName,
                Parameters.Options.Checkers.GetValueOrDefault(property.Name,
                    Parameters.Options.Checkers.GetValueOrDefault("*", new DefaultChecker())));

            var result = checker!.Check(new AssertThatParameters
            {
                Actual = property.GetValue(Parameters.Actual),
                Expected = Parameters.Expected,
                ExpectedType = expectedType,
                PropertyName = property.Name,
                FullPropertyName = newFullPropertyName,
                Options = Parameters.Options
            });
            if (!checker.FoundMatchableProperty)
                missingActualProperties.Add(property.Name);
            checkedExpectedProperties.AddRange(checker.CheckedProperties);
            if (result != null)
                return result;
        }

        if (Parameters.Options.ReportMissingProperties.HasFlag(Direction.Actual) && missingActualProperties.Count > 0)
        {
            var propertyNames = string.Join(", ", missingActualProperties);
            return $"Properties [{propertyNames}] are in {Parameters.PropertyName} but not in {expectedType.Name}";
        }

        if (Parameters.Options.ReportMissingProperties.HasFlag(Direction.Expected))
        {
            var missingProperties = expectedType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => x.Name)
                .Where(property => !checkedExpectedProperties.Contains(property))
                .Where(property => !IsIgnored(property))
                .ToList();
            if (missingProperties.Count != 0)
            {
                var propertyNames = string.Join(", ", missingProperties);
                return $"Properties [{propertyNames}] are in {expectedType.Name} but not in {actualType.Name}";
            }
        }

        return null;
    }

    private bool IsIgnored(string property)
    {
        var checker = Parameters.Options.Checkers.GetValueOrDefault(property);
        return checker switch
        {
            MultiValidator multi => multi.Contains<IgnoredItemChecker>(),
            IgnoredItemChecker => true,
            _ => false
        };
    }
}