using System.Collections;
using System.Reflection;

namespace AssertThatLibrary;

internal abstract class BaseValidator : BaseChecker, IValidator
{
    protected AssertThatParameters Parameters { get; set; } = new();

    public string? Check(AssertThatParameters parameters)
    {
        var actual = parameters.Actual;
        if (actual is not string && actual is IEnumerable actualValue &&
            parameters.Expected is IEnumerable expectedValue)
        {
            var actualList = actualValue.Cast<object>().ToList();
            var expectedList = expectedValue.Cast<object>().ToList();
            if (actualList.Count != expectedList.Count)
                return $"[{actual}] has different number of items than expected";
            var options = parameters.Options;
            if (!options.CompareOrder.GetValueOrDefault(parameters.PropertyName ?? "", true))
            {
                var selectionFunction = options.OrderComparisonFunction;
                actualList = actualList.OrderBy(selectionFunction).ToList();
                expectedList = expectedList.OrderBy(selectionFunction).ToList();
            }

            for (var i = 0; i < actualList.Count; i++)
            {
                var listParameters = new AssertThatParameters(actualList[i], expectedList[i], options);
                var message = Check(listParameters);
                if (message != null)
                    return message + $" at index {i}";
            }
        }
        else
        {
            Parameters = parameters;
            return Check();
        }

        return null;
    }

    protected string CreateMessage() => CreateMessage(Parameters.PropertyName, Parameters.Actual, Parameters.Expected);

    protected abstract string? Check();

    protected string FullPropertyName(PropertyInfo property) =>
        Parameters.PropertyName == null
            ? property.Name
            : $"{Parameters.PropertyName}.{property.Name}";

    protected bool CheckProperty(PropertyInfo property) => property.CanWrite || Parameters.Options.CheckReadOnly.GetValueOrDefault(property.Name, true);
}