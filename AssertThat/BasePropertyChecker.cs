using System;
using System.Linq;

namespace AssertThatLibrary;

public abstract class BasePropertyChecker : BaseChecker, IChecker
{
    public IEnumerable<string> CheckedProperties { get; private set; } = new List<string>();
    public bool FoundMatchableProperty { get; protected set; }

    public virtual string? Check(AssertThatParameters parameters)
    {
        CheckedProperties = new List<string> { parameters.PropertyName! };
        var matches = SimpleCompare(parameters);
        if (matches.HasValue)
        {
            FoundMatchableProperty = true;
            return matches.Value ? null : CreateMessage(parameters.FullPropertyName, parameters.Actual, parameters.Expected);
        }
        return CheckNotNull(new AssertThatNonNullParameters
        {
            Actual = parameters.Actual!,
            ActualType = parameters.ActualType,
            Expected = parameters.Expected!,
            ExpectedType = parameters.ExpectedType,
            FullPropertyName = parameters.FullPropertyName,
            Options = parameters.Options,
            PropertyName = parameters.PropertyName,
        });
    }

    protected abstract string? CheckNotNull(AssertThatNonNullParameters parameters);
}