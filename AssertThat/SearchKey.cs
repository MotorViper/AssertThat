using System.Text.RegularExpressions;

namespace AssertThatLibrary;

public class SearchKey
{
    private readonly string _key;

    public SearchKey(string key)
    {
        _key = key;
        Regex = null;
    }

    public SearchKey(Regex key)
    {
        _key = "";
        Regex = key;
    }

    public Regex? Regex { get; }

    public static SearchKey Create(string key)
    {
        if (key == null)
            throw new SearchKeyException("Search key must not be null");

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

    public bool Equals(string other) => Regex?.IsMatch(other) ?? _key == other;

    public static implicit operator SearchKey(string key) => Create(key);
}