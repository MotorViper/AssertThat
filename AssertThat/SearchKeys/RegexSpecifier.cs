using System.Text.RegularExpressions;

namespace AssertThatLibrary.SearchKeys;

/// <summary>
///     An ISpecifier for a SearchKey that does a regular expression match.
/// </summary>
public class RegexSpecifier : ISpecifier
{
    private readonly Regex _regex;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="regex">The regular expression.</param>
    public RegexSpecifier(Regex regex)
    {
        _regex = regex;
    }

    /// <inheritdoc />
    public bool Equals(string? other) => _regex.IsMatch(other!);
}