using System.Text.RegularExpressions;

namespace AssertThatLibrary;

/// <summary>
///     Class to allow keyword matching using a given method,
///     which can be any of straight text, regular expression, wildcard (with or without ?) or numeric wildcard.
/// </summary>
public class SearchKey : IEquatable<string>
{
    private readonly string _key;

    /// <summary>
    ///     Creates a search key for straight text matching.
    /// </summary>
    /// <param name="key">The key which cannot be null.</param>
    public SearchKey(string key)
    {
        _key = key ?? throw new SearchKeyException("Search key must not be null");
        Regex = null;
    }

    /// <summary>
    ///     Creates a search key with a regular expression.
    /// </summary>
    /// <param name="key">The regular expression which cannot be null.</param>
    public SearchKey(Regex key)
    {
        _key = "";
        Regex = key ?? throw new SearchKeyException("Search key must not be null");
    }

    /// <summary>
    ///     The equivalent regular expression, if relevant.
    /// </summary>
    public Regex? Regex { get; }

    /// <inheritdoc />
    public bool Equals(string? other) => other != null && (Regex?.IsMatch(other) ?? _key == other);

    /// <summary>
    ///     Create a search key. The key can have any of the following formats:
    ///     "xxx" - straight text match
    ///     "D:xxx" - a numeric wildcard match, '*' = any character, '?' = single character, '#' - numeric character
    ///     "R:xxx" - a regular expression match
    ///     "S:xxx" - a simple wildcard match, '*' = any character
    ///     "T:xxx" - straight text match
    ///     "W:xxx" - a normal wildcard match, '*' = any character, '?' = single character
    /// </summary>
    /// <param name="key">The key string.</param>
    /// <returns>The search key.</returns>
    public static SearchKey Create(string key)
    {
        char specifier;
        string actualKey;
        if (key.Length > 2 && key[1] == ':')
        {
            specifier = key[0];
            actualKey = key[2..];
        }
        else
        {
            specifier = 'T';
            actualKey = key;
        }

        return specifier switch
        {
            'D' => new SearchKey(
                new Regex(AddEnds(ConvertHash(ConvertStar(ConvertQuestionMark(Regex.Escape(actualKey))))))),
            'R' => new SearchKey(new Regex(actualKey)),
            'S' => new SearchKey(new Regex(AddEnds(ConvertStar(Regex.Escape(actualKey))))),
            'T' => new SearchKey(actualKey),
            'W' => new SearchKey(new Regex(AddEnds(ConvertStar(ConvertQuestionMark(Regex.Escape(actualKey)))))),
            _ => throw new SearchKeyException(
                $"Search key specifier '{specifier}' is invalid, must be one of D, R, S, T or W")
        };
    }

    private static string AddEnds(string text) => "^" + text + "$";

    private static string ConvertHash(string text) => text.Replace("\\#", "[0-9]+");

    private static string ConvertQuestionMark(string text) => text.Replace("\\?", ".");

    private static string ConvertStar(string text) => text.Replace("\\*", ".*");

    /// <summary>
    ///     Allows search keys to automatically be created from strings.
    ///     See SearchKey.Create for further information.
    /// </summary>
    /// <param name="key">The string to convert.</param>
    /// <returns>A search key.</returns>
    public static implicit operator SearchKey(string key) => Create(key);
}