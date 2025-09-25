using System.Linq.Expressions;

namespace AssertThatLibrary;

/// <summary>
///     Base class for providing assertions.
/// </summary>
/// <typeparam name="TActual">The type of the item being checked.</typeparam>
public class AssertThat<TActual>
{
    private readonly TActual? _actual;
    private AssertThatOptions _options = new();

    internal AssertThat(TActual? actual)
    {
        _actual = actual;
    }

    /// <summary>
    ///     Checks that the item being checked is of the correct type.
    /// </summary>
    /// <typeparam name="TExpected">The type the item should be.</typeparam>
    public void Is<TExpected>()
    {
        if (typeof(TActual) == typeof(TExpected))
            return;
        if (_actual == null)
            throw new AssertThatException("Test value is null so cannot check type");
        if (_options.Exact || typeof(TActual) != typeof(object) || _actual.GetType() != typeof(TExpected))
            throw new AssertThatException($"Test value [{_actual}] is not of type {typeof(TExpected).Name}");
    }

    /// <summary>
    ///     Checks that the item being checked has the correct value.
    ///     This will use existing equality checks if they exist.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    public void Is(TActual? expected) => WithExact(true).IsLike(expected);

    /// <summary>
    ///     Checks that the item being checked is not of the given type.
    /// </summary>
    /// <typeparam name="TExpected">The type the item should not be.</typeparam>
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

    /// <summary>
    ///     Checks that the item being checked does not have the given value.
    ///     This will use existing equality checks if they exist.
    /// </summary>
    /// <param name="notExpected">The value that the item should not have.</param>
    public void IsNot(TActual? notExpected) => WithExact(true).IsNotLike(notExpected);

    /// <summary>
    ///     Checks that the test item is in a collection.
    /// </summary>
    /// <param name="items">The collection of items to check.</param>
    public void IsIn(params TActual[] items)
    {
        if (items.Select(item => Check(new AssertThatParameters(_actual, item, _options, StopWhen.Match)))
            .All(message => message != null))
            throw new AssertThatException($"[{_actual}] is not in collection");
    }

    /// <summary>
    ///     Checks that the test item is not in a collection.
    /// </summary>
    /// <param name="items">The collection of items to check.</param>
    public void IsNotIn(params TActual[] items)
    {
        if (items.Select(item => Check(new AssertThatParameters(_actual, item, _options, StopWhen.Match)))
            .Any(message => message == null))
            throw new AssertThatException($"[{_actual}] is in collection");
    }

    /// <summary>
    ///     Checks that the item being checked has the correct value.
    ///     If the actual and expected types are different then properties with the same name are checked.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    public void IsLike(object? expected)
    {
        var message = Check(new AssertThatParameters(_actual, expected, _options, StopWhen.NotMatch));
        if (message != null)
            throw new AssertThatException(message);
    }

    /// <summary>
    ///     Checks that the item being checked does not have the given value.
    ///     If the actual and expected types are different then properties with the same name are checked.
    /// </summary>
    /// <param name="notExpected">The value that the item should not have.</param>
    public void IsNotLike(object? notExpected)
    {
        var message = Check(new AssertThatParameters(_actual, notExpected, _options, StopWhen.Match));
        if (message == null)
            throw new AssertThatException("Actual and expected values are the same");
    }

    public AssertThat<TActual> WithAutoConvert() => WithCustomChecker("*", new AutoConvertChecker());

    public AssertThat<TActual> WithAutoConvertFor(SearchKey propertyName) =>
        WithCustomChecker(propertyName, new AutoConvertChecker());

    public AssertThat<TActual> WithAutoConvertFor(IEnumerable<string> propertyNames)
    {
        foreach (var propertyName in propertyNames)
            WithAutoConvertFor(propertyName);
        return this;
    }

    public AssertThat<TActual> WithAutoConvertFor(params string[] propertyNames) =>
        WithAutoConvertFor(propertyNames.ToList());

    public AssertThat<TActual> WithCheckReadOnly(string propertyName)
    {
        WithDontCheckReadOnly();
        _options.CheckReadOnly.Add(propertyName);
        return this;
    }

    public AssertThat<TActual> WithCheckReadOnly(IEnumerable<string> propertyNames)
    {
        foreach (var propertyName in propertyNames)
            WithCheckReadOnly(propertyName);
        return this;
    }

    public AssertThat<TActual> WithCheckReadOnly(params string[] propertyNames) =>
        WithCheckReadOnly(propertyNames.ToList());

    public AssertThat<TActual> WithDontCheckReadOnly()
    {
        if (_options.CheckReadOnly.Contains("*"))
            _options.CheckReadOnly.Remove("*");
        return this;
    }

    public AssertThat<TActual> WithCustomChecker(SearchKey propertyName, IChecker checker)
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

    public AssertThat<TActual> WithCustomChecker(SearchKey propertyName,
        Func<AssertThatParameters, string?> checker) =>
        WithCustomChecker(propertyName, new CustomChecker(checker));

    public AssertThat<TActual> WithEquivalentProperty(SearchKey propertyName, string equivalentPropertyName) =>
        WithCustomChecker(propertyName, new EquivalentPropertyChecker(equivalentPropertyName));

    public AssertThat<TActual> WithEquivalentProperty<TExpected>(Expression<Func<TActual, object>> func,
        Expression<Func<TExpected, object>> equivalentFunc) =>
        WithEquivalentProperty(GetNameFromExpression(func), GetNameFromExpression(equivalentFunc));

    private string GetNameFromExpression<T>(Expression<Func<T, object>> func)
    {
        if (func.Body is MemberExpression expression)
            return expression.Member.Name;

        var unary = func.Body as UnaryExpression;
        var operand = unary.Operand as MemberExpression;
        return operand.Member.Name;
    }

    public AssertThat<TActual> WithEquivalentProperties(Dictionary<SearchKey, string> equivalences)
    {
        foreach (var equivalence in equivalences)
            WithEquivalentProperty(equivalence.Key, equivalence.Value);
        return this;
    }

    public AssertThat<TActual> WithEquivalentProperties(params (SearchKey, string)[] equivalences) =>
        WithEquivalentProperties(equivalences.ToDictionary());

    public AssertThat<TActual> WithExact(bool value)
    {
        _options = _options with { Exact = value };
        return this;
    }

    public AssertThat<TActual> WithIgnoredProperty(SearchKey propertyName) =>
        WithCustomChecker(propertyName, new IgnoredItemChecker());

    public AssertThat<TActual> WithIgnoredProperties(IEnumerable<SearchKey> propertyNames)
    {
        foreach (var propertyName in propertyNames)
            WithIgnoredProperty(propertyName);
        return this;
    }

    public AssertThat<TActual> WithIgnoredProperties(params SearchKey[] propertyNames) =>
        WithIgnoredProperties(propertyNames.ToList());

    public AssertThat<TActual> WithOptions(AssertThatOptions options)
    {
        _options = options ?? new AssertThatOptions();
        return this;
    }

    public AssertThat<TActual> WithReportMissing(Direction direction)
    {
        _options = _options with { ReportMissingProperties = direction };
        return this;
    }

    private IValidator CreateValidator(AssertThatParameters parameters) =>
        _options.Exact || parameters.ActualType == parameters.ExpectedType
            ? new IsValidator()
            : new IsLikeValidator();

    private string? Check(AssertThatParameters parameters)
    {
        var checker = CreateValidator(parameters);
        var message = checker.Check(parameters);
        return message;
    }
}

public static class AssertThat
{
    public static AssertThat<TActual> TestValue<TActual>(TActual item) => new(item);
}