namespace AssertThatLibrary;

public class MultiValidator : IChecker
{
    public List<IChecker> Children { get; } = new();

    public IEnumerable<string> CheckedProperties
    {
        get { return Children.SelectMany(x => x.CheckedProperties); }
    }

    public bool FoundMatchableProperty { get; private set; } = true;

    public string? Check(AssertThatParameters parameters)
    {
        foreach (var child in Children)
        {
            var message = child.Check(parameters);
            if (child.FoundMatchableProperty)
                FoundMatchableProperty = true;
            if (message != null)
                return message;
        }

        return null;
    }

    public bool Contains<T>()
    {
        foreach (var child in Children)
            switch (child)
            {
                case T:
                    return true;
                case MultiValidator multi:
                {
                    var found = multi.Contains<T>();
                    if (found)
                        return true;
                    break;
                }
            }

        return false;
    }
}