namespace AssertThatLibrary.SearchKeys;

/// <summary>
///     An ISpecifier for a general SearchKey.
/// </summary>
public class SearchKeySpecifier : ISpecifier
{
    private readonly string _key;
    private readonly Func<string, string?, bool> _matcher;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="key">The text to match against.</param>
    /// <param name="matcher">How the text should be matched.</param>
    public SearchKeySpecifier(string key, Func<string, string?, bool> matcher, bool isExactMatch = true)
    {
        _key = key;
        _matcher = matcher;
        IsExactMatch = isExactMatch;
    }

    /// <inheritdoc />
    public bool Equals(string? other) => _matcher(_key, other);

    /// <inheritdoc />
    public bool IsExactMatch { get; }
}