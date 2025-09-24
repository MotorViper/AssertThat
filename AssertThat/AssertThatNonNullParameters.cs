using System;
using System.Linq;

namespace AssertThatLibrary;

public record AssertThatNonNullParameters
{
    public required object Actual { get; init; }
    public required Type ActualType { get; init; }
    public required object Expected { get; init; }
    public required Type ExpectedType { get; init; }
    public required AssertThatOptions Options { get; init; }
    public required string? PropertyName { get; init; }
    public required string? FullPropertyName { get; init; }
}
