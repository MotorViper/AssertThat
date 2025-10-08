using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AssertThatLibrary.SearchKeys;

/// <summary>
///     Class to allow keyword matching using a given method,
///     which can be any of straight text, regular expression, wildcard (with or without ?) or numeric wildcard.
/// </summary>
public class SearchKey : IEquatable<string>
{
    private readonly ISpecifier _specifier;

    /// <summary>
    ///     Creates a search key for straight text matching.
    /// </summary>
    /// <param name="key">The key which cannot be null.</param>
    public SearchKey(string key)
    {
        _specifier = new SearchKeySpecifier(key, (s1, s2) => s1.Equals(s2));
    }

    /// <summary>
    ///     Creates a search key with a regular expression.
    /// </summary>
    /// <param name="key">The regular expression which cannot be null.</param>
    public SearchKey(Regex key)
    {
        _specifier = new RegexSpecifier(key);
    }

    private SearchKey(ISpecifier specifier)
    {
        _specifier = specifier;
    }

    /// <summary>
    ///     The function to create the ISpecifier to use when there is no indicator.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static Func<string, ISpecifier> DefaultSpecifier { get; set; } = x => new SearchKeySpecifier(x, (s1, s2) => s1.Equals(s2));

    /// <summary>
    ///     Functions to create specifiers for each indicator.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static Dictionary<char, Func<string, ISpecifier>> Specifiers { get; } = new()
    {
        ['D'] = x => new RegexSpecifier(new Regex(AddEnds(ConvertHash(ConvertStar(ConvertQuestionMark(Regex.Escape(x))))))),
        ['R'] = x => new RegexSpecifier(new Regex(x)),
        ['S'] = x => new RegexSpecifier(new Regex(AddEnds(ConvertStar(Regex.Escape(x))))),
        ['T'] = x => new SearchKeySpecifier(x, (s1, s2) => s1.Equals(s2)),
        ['W'] = x => new RegexSpecifier(new Regex(AddEnds(ConvertStar(ConvertQuestionMark(Regex.Escape(x))))))
    };

    /// <inheritdoc />
    public bool Equals(string? other) => _specifier.Equals(other);

    /// <summary>
    ///     Create a search key. The key can have any of the following formats:
    ///     "xxx" - straight text match - this can be replaced using the DefaultSpecifier static property.
    ///     "D:xxx" - a numeric wildcard match, '*' = any character, '?' = single character, '#' - numeric character
    ///     "R:xxx" - a regular expression match
    ///     "S:xxx" - a simple wildcard match, '*' = any character
    ///     "T:xxx" - straight text match
    ///     "W:xxx" - a normal wildcard match, '*' = any character, '?' = single character
    ///     These can be changed using Specifiers static property.
    /// </summary>
    /// <param name="key">The key string.</param>
    /// <returns>The search key.</returns>
    public static SearchKey Create(string key)
    {
        ISpecifier specifier;
        if (key.Length > 2 && key[1] == ':')
        {
            if (Specifiers.TryGetValue(key[0], out var specifierFunc))
                specifier = specifierFunc(key[2..]);
            else
                throw new SearchKeyException(
                    $"Search key specifier '{key[0]}' is invalid, must be one of {Specifiers.Keys}");
        }
        else
        {
            specifier = DefaultSpecifier(key);
        }

        return new SearchKey(specifier);
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