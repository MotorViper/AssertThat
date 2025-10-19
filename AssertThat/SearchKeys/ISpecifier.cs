namespace AssertThatLibrary.SearchKeys;

/// <summary>
///     Used to create a SearchKey.
/// </summary>
public interface ISpecifier : IEquatable<string>
{
    /// <summary>
    ///     True if an exact match, false if a regular expression type match.
    /// </summary>
    bool IsExactMatch { get; }
}