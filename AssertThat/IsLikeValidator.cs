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
        foreach (var property in properties)
            if (CheckProperty(property))
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
            else
            {
                checkedExpectedProperties.Add(property.Name);
            }

        if (Parameters.Options.ReportMissingProperties.HasFlag(Direction.Actual) && missingActualProperties.Count > 0)
            return CreateMissingPropertiesMessage(missingActualProperties, actualType, expectedType);

        if (Parameters.Options.ReportMissingProperties.HasFlag(Direction.Expected))
        {
            var missingProperties = expectedType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => x.Name)
                .Where(property => !checkedExpectedProperties.Contains(property))
                .Where(property => !IsIgnored(property))
                .ToList();
            if (missingProperties.Count != 0)
                return CreateMissingPropertiesMessage(missingProperties, expectedType, actualType);
        }

        return null;
    }

    private static string? CreateMissingPropertiesMessage(List<string> missingProperties, Type inType, Type notInType)
    {
        if (missingProperties.Count == 1)
            return $"Property [{missingProperties[0]}] is in {inType.Name} but not in {notInType.Name}";
        var propertyNames = string.Join(", ", missingProperties);
        return $"Properties [{propertyNames}] are in {inType.Name} but not in {notInType.Name}";
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