using System.Linq.Expressions;
using AssertThatLibrary.SearchKeys;

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
        var actualType = _actual.GetType();
        if (_options.Exact || _actual is not TExpected)
            throw new AssertThatException(
                $"Test value type [{actualType.Name}] is not of type [{typeof(TExpected).Name}]");
    }

    /// <summary>
    ///     Checks that the item being checked has the correct value.
    ///     This will use existing equality checks if they exist.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    public void Is(TActual? expected)
    {
        WithExact().IsLike(expected);
    }

    /// <summary>
    ///     Checks that the item being checked is not of the given type.
    /// </summary>
    /// <typeparam name="TExpected">The type the item should not be.</typeparam>
    public void IsNot<TExpected>()
    {
        if (typeof(TActual) == typeof(TExpected))
            throw new AssertThatException($"Test value is of type [{typeof(TExpected).Name}]");
        if (_actual == null)
            throw new AssertThatException("Test value is null so cannot check type");
        if (_options.Exact)
            return;
        if (_actual is TExpected)
            throw new AssertThatException($"Test value is of type [{typeof(TExpected).Name}]");
    }

    /// <summary>
    ///     Checks that the item being checked does not have the given value.
    ///     This will use existing equality checks if they exist.
    /// </summary>
    /// <param name="notExpected">The value that the item should not have.</param>
    public void IsNot(TActual? notExpected)
    {
        WithExact().IsNotLike(notExpected);
    }

    /// <summary>
    ///     Checks that the test item is in a collection.
    /// </summary>
    /// <param name="items">The collection of items to check.</param>
    public void IsIn(params TActual[] items)
    {
        if (items.Select(item => Check(new AssertThatParameters(_actual, item, _options)))
            .All(message => message != null))
            throw new AssertThatException($"[{_actual}] is not in collection");
    }

    /// <summary>
    ///     Checks that the test item is not in a collection.
    /// </summary>
    /// <param name="items">The collection of items to check.</param>
    public void IsNotIn(params TActual[] items)
    {
        if (items.Select(item => Check(new AssertThatParameters(_actual, item, _options)))
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
        var message = Check(new AssertThatParameters(_actual, expected, _options));
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
        var message = Check(new AssertThatParameters(_actual, notExpected, _options));
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

    /// <summary>
    ///     Whether to compare ordering of values in collections.
    ///     If not a collection this is ignored.
    /// </summary>
    /// <param name="compare">Whether to compare ordering.</param>
    /// <returns>The current object.</returns>
    public AssertThat<TActual> WithCompareOrder(bool compare = true) => WithCompareOrder("W:*", compare);

    /// <summary>
    ///     Whether to compare ordering of values in collections.
    ///     If not a collection this is ignored.
    /// </summary>
    /// <param name="propertyName">The property to check ordering for.</param>
    /// <param name="compare">Whether to compare ordering.</param>
    /// <returns>The current object.</returns>
    public AssertThat<TActual> WithCompareOrder(SearchKey propertyName, bool compare = true)
    {
        _options.CompareOrder.Add(propertyName, compare);
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

    public AssertThat<TActual> WithEquivalentProperties(Dictionary<SearchKey, string> equivalences)
    {
        foreach (var equivalence in equivalences)
            WithEquivalentProperty(equivalence.Key, equivalence.Value);
        return this;
    }

    public AssertThat<TActual> WithEquivalentProperties(params (SearchKey, string)[] equivalences) =>
        WithEquivalentProperties(equivalences.ToDictionary());

    /// <summary>
    ///     If exact is true the actual and expected values must be equatable.
    ///     If the type is being checked this is further constrained to be of exact same type.
    /// </summary>
    /// <param name="value">Whether the check should be exact.</param>
    /// <returns>The current object.</returns>
    public AssertThat<TActual> WithExact(bool value = true)
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

    private string GetNameFromExpression<T>(Expression<Func<T, object>> func)
    {
        if (func.Body is MemberExpression expression)
            return expression.Member.Name;

        var unary = func.Body as UnaryExpression;
        var operand = unary.Operand as MemberExpression;
        return operand.Member.Name;
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

/// <summary>
///     Class used as basis of fluent creation of test case.
/// </summary>
public static class AssertThat
{
    public static AssertThat<TActual> TestValue<TActual>(TActual item) => new(item);
}