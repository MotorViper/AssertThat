using System;
using System.Linq;

namespace AssertThatLibrary;

public interface IChecker : IValidator
{
    IEnumerable<string> CheckedProperties { get; }
    bool FoundMatchableProperty { get; }
}

public interface IValidator
{
    string? Check(AssertThatParameters parameters);
}
