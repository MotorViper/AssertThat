using System.Reflection;

namespace AssertThatLibrary;

internal abstract class BaseValidator : BaseChecker, IValidator
{
    protected AssertThatParameters Parameters { get; set; } = new AssertThatParameters();

    public string CreateMessage()
    {
        return CreateMessage(Parameters.PropertyName, Parameters.Actual, Parameters.Expected);
    }

    public string? Check(AssertThatParameters parameters)
    {
        Parameters = parameters;
        return Check();
    }

    protected virtual string? Check() => null;

    protected string FullPropertyName(PropertyInfo property) =>
        Parameters.PropertyName == null ? property.Name : $"{Parameters.PropertyName}.{property.Name}";

    protected IEnumerable<PropertyInfo> GetPropertiesToCheck(PropertyInfo[] properties)
    {
        return properties.Where(x => x.CanWrite || Parameters.Options.CheckReadOnly.Contains("*") ||
            Parameters.Options.CheckReadOnly.Contains(x.Name));
    }
}