using System.Text.RegularExpressions;

namespace AssertThatLibrary;

public class SearchKey
{
    private readonly string _key;
    private readonly Regex _regex;

    public SearchKey(string key)
    {
        if (key.StartsWith("S:"))
            _regex = new Regex("^" + Regex.Escape(key.Substring(2)).Replace("\\*", ".*") + "$");
        else if (key.StartsWith("W:"))
            _regex = new Regex("^" + Regex.Escape(key.Substring(2)).Replace("\\?", ".").Replace("\\*", ".*") + "$");
        else if (key.StartsWith("D:"))
            _regex = new Regex("^" + Regex.Escape(key.Substring(2)).Replace("\\?", ".").Replace("\\*", ".*")
                .Replace("#", "[0-9]+") + "$");
        else if (key.StartsWith("R:"))
            _regex = new Regex(key.Substring(2));
        else
            _key = key;
    }

    public bool Equals(string other)
    {
        return _key != null ? _key == other : _regex.IsMatch(other);
    }

    public static implicit operator SearchKey(string key)
    {
        return new SearchKey(key);
    }
}