namespace AssertThatLibrary;

public class Checkers
{
    private readonly Dictionary<SearchKey, IChecker> _checkers = new();

    public IChecker? GetValueOrDefault(string propertyName, IChecker? defaultValue = null)
    {
        foreach (var checker in _checkers)
            if (checker.Key.Equals(propertyName))
                return checker.Value;
        return defaultValue;
    }

    public bool TryGetValue(SearchKey propertyName, out IChecker checker)
    {
        return _checkers.TryGetValue(propertyName, out checker!);
    }

    public void Add(SearchKey propertyName, IChecker checker)
    {
        _checkers.Add(propertyName, checker);
    }
}