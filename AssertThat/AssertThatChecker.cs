using System.Collections;

namespace AssertThatLibrary;

public class AssertThatChecker<TActual>
{
    private readonly TActual? _actual;
    private AssertThatOptions _options = new();

    internal AssertThatChecker(TActual? actual)
    {
        _actual = actual;
    }

    public void Is<TExpected>()
    {
        if (typeof(TActual) == typeof(TExpected))
            return;
        if (_actual == null)
            throw new AssertThatException("Test value is null so cannot check type");
        if (_options.Exact || typeof(TActual) != typeof(object) || _actual.GetType() != typeof(TExpected))
            throw new AssertThatException($"Test value [{_actual}] is not of type {typeof(TExpected).Name}");
    }

    public void Is(TActual? expected)
    {
        WithExact(true).IsLike(expected);
    }

    public void IsNot<TExpected>()
    {
        if (typeof(TActual) == typeof(TExpected))
            throw new AssertThatException($"Test value [{_actual}] is of type {typeof(TExpected).Name}");
        if (_actual == null)
            throw new AssertThatException("Test value is null so cannot check type");
        if (_options.Exact)
            return;
        if (typeof(TActual) == typeof(object) && _actual.GetType() == typeof(TExpected))
            throw new AssertThatException($"Test value [{_actual}] is of type {typeof(TExpected).Name}");
    }

    public void IsNot(TActual? notExpected)
    {
        WithExact(true).IsNotLike(notExpected);
    }

    public void IsIn(params TActual[] items)
    {
        foreach (var item in items)
        {
            var checker = new IsValidator();
            var message = checker.Check(new AssertThatParameters(_actual, item, _options));
            if (message == null)
                return;
        }

        throw new AssertThatException($"[{_actual}] is not in collection");
    }

    public void SimilarIsIn<TExpected>(params TExpected[] items)
    {
        foreach (var item in items)
        {
            var checker = new IsLikeValidator();
            var message = Check(checker, _actual, item, _options, StopWhen.NotMatch);
            if (message == null)
                return;
        }

        throw new AssertThatException($"[{_actual}] is not in collection");
    }

    public void IsNotIn(params TActual[] items)
    {
        foreach (var item in items)
        {
            var checker = new IsValidator();
            var message = checker.Check(new AssertThatParameters(_actual, item, _options));
            if (message == null)
                throw new AssertThatException($"[{_actual}] is in list");
        }
    }

    public void IsLike(object? expected)
    {
        if (_options.Exact)
        {
            var checker = new IsValidator();
            var message = checker.Check(new AssertThatParameters(_actual, expected, _options));
            if (message != null)
                throw new AssertThatException(message);
        }
        else
        {
            var checker = new IsLikeValidator();
            var message = Check(checker, _actual, expected, _options, StopWhen.NotMatch);
            if (message != null)
                throw new AssertThatException(message);
        }
    }

    public void IsNotLike(object? notExpected)
    {
        if (_options.Exact)
        {
            var checker = new IsValidator();
            var message = checker.Check(new AssertThatParameters(_actual, notExpected, _options));
            if (message == null)
                throw new AssertThatException("Actual and expected values are the same");
        }
        else
        {
            var checker = new IsLikeValidator();
            var message = Check(checker, _actual, notExpected, _options, StopWhen.Match);
            if (message == null)
                throw new AssertThatException("Actual and expected values are the same");
        }
    }

    private string? Check(BaseValidator checker, object? actual, object? expected, AssertThatOptions? options,
        StopWhen when)
    {
        if (actual is IEnumerable actualValue && expected is IEnumerable expectedValue)
        {
            var actualList = actualValue.Cast<object>().ToList();
            var expectedList = expectedValue.Cast<object>().ToList();
            if (actualList.Count != expectedList.Count)
                return $"[{_actual}] has different number of items than expected";
            if (!_options.CompareOrder ?? true)
            {
                var selectionFunction = _options.OrderComparisonFunction ?? (x => x.ToString() ?? string.Empty);
                actualList = actualList.OrderBy(selectionFunction).ToList();
                expectedList = expectedList.OrderBy(selectionFunction).ToList();
            }

            for (var i = 0; i < actualList.Count; i++)
            {
                var listParameters = new AssertThatParameters(actualList[i], expectedList[i], _options);
                var message = checker.Check(listParameters);
                if ((message != null && when == StopWhen.NotMatch) || (message == null && when == StopWhen.Match))
                    return message;
            }
        }
        else
        {
            return checker.Check(new AssertThatParameters(actual, expected, options));
        }

        return null;
    }

    public AssertThatChecker<TActual> WithAutoConvert()
    {
        return WithCustomChecker("*", new AutoConvertChecker());
    }

    public AssertThatChecker<TActual> WithAutoConvertFor(SearchKey propertyName)
    {
        return WithCustomChecker(propertyName, new AutoConvertChecker());
    }

    public AssertThatChecker<TActual> WithAutoConvertFor(IEnumerable<string> propertyNames)
    {
        foreach (var propertyName in propertyNames)
            WithAutoConvertFor(propertyName);
        return this;
    }

    public AssertThatChecker<TActual> WithAutoConvertFor(params string[] propertyNames)
    {
        return WithAutoConvertFor(propertyNames.ToList());
    }

    public AssertThatChecker<TActual> WithCheckReadOnly(string propertyName)
    {
        WithDontCheckReadOnly();
        _options.CheckReadOnly.Add(propertyName);
        return this;
    }

    public AssertThatChecker<TActual> WithCheckReadOnly(IEnumerable<string> propertyNames)
    {
        foreach (var propertyName in propertyNames)
            WithCheckReadOnly(propertyName);
        return this;
    }

    public AssertThatChecker<TActual> WithCheckReadOnly(params string[] propertyNames)
    {
        return WithCheckReadOnly(propertyNames.ToList());
    }

    public AssertThatChecker<TActual> WithDontCheckReadOnly()
    {
        if (_options.CheckReadOnly.Contains("*"))
            _options.CheckReadOnly.Remove("*");
        return this;
    }

    public AssertThatChecker<TActual> WithCustomChecker(SearchKey propertyName, IChecker checker)
    {
        if (_options.Checkers.TryGetValue(propertyName, out var existing))
        {
            if (existing is not MultiValidator validator)
            {
                validator = new MultiValidator();
                validator.Children.Add(existing);
            }

            validator.Children.Add(checker);
        }
        else
        {
            _options.Checkers.Add(propertyName, checker);
        }

        return this;
    }

    public AssertThatChecker<TActual> WithCustomChecker(SearchKey propertyName,
        Func<AssertThatParameters, string?> checker)
    {
        return WithCustomChecker(propertyName, new CustomChecker(checker));
    }

    public AssertThatChecker<TActual> WithEquivalentProperty(SearchKey propertyName, string equivalentPropertyName)
    {
        return WithCustomChecker(propertyName, new EquivalentPropertyChecker(equivalentPropertyName));
    }

    public AssertThatChecker<TActual> WithEquivalentProperties(Dictionary<SearchKey, string> equivalences)
    {
        foreach (var equivalence in equivalences)
            WithEquivalentProperty(equivalence.Key, equivalence.Value);
        return this;
    }

    public AssertThatChecker<TActual> WithEquivalentProperties(params (SearchKey, string)[] equivalences)
    {
        return WithEquivalentProperties(equivalences.ToDictionary());
    }

    public AssertThatChecker<TActual> WithExact(bool value)
    {
        _options = _options with { Exact = value };
        return this;
    }

    public AssertThatChecker<TActual> WithIgnoredProperty(SearchKey propertyName)
    {
        return WithCustomChecker(propertyName, new IgnoredItemChecker());
    }

    public AssertThatChecker<TActual> WithIgnoredProperties(IEnumerable<SearchKey> propertyNames)
    {
        foreach (var propertyName in propertyNames)
            WithIgnoredProperty(propertyName);
        return this;
    }

    public AssertThatChecker<TActual> WithIgnoredProperties(params SearchKey[] propertyNames)
    {
        return WithIgnoredProperties(propertyNames.ToList());
    }

    public AssertThatChecker<TActual> WithOptions(AssertThatOptions options)
    {
        _options = options ?? new AssertThatOptions();
        return this;
    }

    public AssertThatChecker<TActual> WithReportMissing(Direction direction)
    {
        _options = _options with { ReportMissingProperties = direction };
        return this;
    }
}