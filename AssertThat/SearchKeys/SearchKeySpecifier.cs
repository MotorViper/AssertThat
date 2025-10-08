namespace AssertThatLibrary.SearchKeys;

/// <summary>
/// An ISpecifier for a general SearchKey.
/// </summary>
public class SearchKeySpecifier : ISpecifier
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="key">The text to match against.</param>
    /// <param name="matcher">How the text should be matched.</param>
    public SearchKeySpecifier(string key, Func<string, string?, bool> matcher)
    {
        _key = key;
        _matcher = matcher;
    }

    private readonly string _key;
    private readonly Func<string, string?, bool> _matcher;

    /// <inheritdoc />
    public bool Equals(string? other) => _matcher(_key, other);
}