using System.Reflection;

namespace AssertThatLibrary;

internal abstract class BaseValidator : BaseChecker, IValidator
{
    protected AssertThatParameters Parameters { get; set; } = new();

    public virtual string? Check(AssertThatParameters parameters)
    {
        Parameters = parameters;
        return Check();
    }

    public string CreateMessage() => CreateMessage(Parameters.PropertyName, Parameters.Actual, Parameters.Expected);

    protected virtual string? Check() => null;

    protected string FullPropertyName(PropertyInfo property) => Parameters.PropertyName == null
        ? property.Name
        : $"{Parameters.PropertyName}.{property.Name}";

    protected IEnumerable<PropertyInfo> GetPropertiesToCheck(PropertyInfo[] properties) =>
        properties.Where(x => x.CanWrite || Parameters.Options.CheckReadOnly.Contains("*") ||
                              Parameters.Options.CheckReadOnly.Contains(x.Name));
}