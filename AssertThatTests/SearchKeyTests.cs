using System.Collections;
using System.Text.RegularExpressions;
using AssertThatLibrary;
using AssertThatLibrary.SearchKeys;
using JetBrains.Annotations;

namespace AssertThatTests;

[TestSubject(typeof(SearchKey))]
public class SearchKeyTests
{
    [Theory]
    [ClassData(typeof(TestData))]
    public void CheckTextOnly(string key, bool[] matches)
    {
        var searchKey = SearchKey.Create(key);
        AssertThat.TestValue(searchKey.Equals("Value")).Is(matches[0]);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void CheckTextOnlyFromConstructor(string key, bool[] matches)
    {
        var searchKey = new SearchKey(key);
        AssertThat.TestValue(searchKey.Equals("Value")).Is(matches[4]);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void CheckRegexpFromConstructor(string key, bool[] matches)
    {
        var searchKey = new SearchKey(new Regex(key, RegexOptions.Compiled));
        AssertThat.TestValue(searchKey.Equals("Value12")).Is(matches[5]);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void CheckSingleCharacter(string key, bool[] matches)
    {
        var searchKey = SearchKey.Create(key);
        AssertThat.TestValue(searchKey.Equals("V")).Is(matches[1]);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void CheckSingleDigit(string key, bool[] matches)
    {
        var searchKey = SearchKey.Create(key);
        AssertThat.TestValue(searchKey.Equals("Value1")).Is(matches[2]);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void CheckMultipleDigits(string key, bool[] matches)
    {
        var searchKey = SearchKey.Create(key);
        AssertThat.TestValue(searchKey.Equals("Value12")).Is(matches[3]);
    }

    private class TestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return ["V", new[] { false, true, false, false, false, true }];
            yield return ["Value", new[] { true, false, false, false, true, true }];
            yield return ["Other", new[] { false, false, false, false, false, false }];
            yield return ["D:Value", new[] { true, false, false, false, false, false }];
            yield return ["D:Value#", new[] { false, false, true, true, false, false }];
            yield return ["D:*", new[] { true, true, true, true, false, false }];
            yield return ["R:^Value[0-9]$", new[] { false, false, true, false, false, false }];
            yield return ["R:^Value[0-9]+$", new[] { false, false, true, true, false, false }];
            yield return ["^Value[0-9]+$", new[] { false, false, false, false, false, true }];
            yield return ["R:^Value$", new[] { true, false, false, false, false, false }];
            yield return ["R:Value", new[] { true, false, true, true, false, false }];
            yield return ["R:.*", new[] { true, true, true, true, false, false }];
            yield return ["S:Value", new[] { true, false, false, false, false, false }];
            yield return ["S:*", new[] { true, true, true, true, false, false }];
            yield return ["T:Value", new[] { true, false, false, false, false, false }];
            yield return ["W:Value", new[] { true, false, false, false, false, false }];
            yield return ["W:*", new[] { true, true, true, true, false, false }];
            yield return ["W:?", new[] { false, true, false, false, false, false }];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}