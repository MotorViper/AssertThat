namespace AssertThatLibrary.SearchKeys;

public class SearchKeyDictionary<T>
{
    private readonly Dictionary<SearchKey, T> _checkers = new();

    public T? GetValueOrDefault(string propertyName, T? defaultValue = default)
    {
        foreach (var checker in _checkers.Where(x => x.Key.IsExactMatch))
            if (checker.Key.Equals(propertyName))
                return checker.Value;
        foreach (var checker in _checkers.Where(x => !x.Key.IsExactMatch))
            if (checker.Key.Equals(propertyName))
                return checker.Value;
        return defaultValue;
    }

    public bool TryGetValue(SearchKey propertyName, out T checker) => _checkers.TryGetValue(propertyName, out checker!);

    public void Add(SearchKey propertyName, T checker)
    {
        _checkers.Add(propertyName, checker);
    }
}