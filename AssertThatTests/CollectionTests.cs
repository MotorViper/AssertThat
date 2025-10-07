using System.Diagnostics.CodeAnalysis;
using AssertThatLibrary;

namespace AssertThatTests;

[SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments")]
public class CollectionTests
{
    [Fact]
    public void TestMatchesArrays()
    {
        AssertThat.TestValue(new[] { 6, 7, 1, 5 }).WithCompareOrder().Is([6, 7, 1, 5]);
        AssertThat.TestValue(new[] { 6, 7, 1, 5 }).WithCompareOrder(false).Is([6, 7, 1, 5]);
        var exception = Assert.Throws<AssertThatException>(() =>
            AssertThat.TestValue(new[] { 6, 7, 1, 5 }).WithCompareOrder().IsNot([6, 7, 1, 5]));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are the same");
        exception = Assert.Throws<AssertThatException>(() =>
            AssertThat.TestValue(new[] { 6, 7, 1, 5 }).WithCompareOrder(false).IsNot([6, 7, 1, 5]));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are the same");
    }

    [Fact]
    public void TestMatchesArraysDifferentOrder()
    {
        var exception = Assert.Throws<AssertThatException>(() =>
            AssertThat.TestValue(new[] { 6, 7, 1, 5 }).WithCompareOrder().Is([6, 5, 7, 1]));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are different: [7] != [5] at index 1");
        AssertThat.TestValue(new[] { 6, 7, 1, 5 }).WithCompareOrder(false).Is([6, 5, 7, 1]);
        AssertThat.TestValue(new[] { 6, 7, 1, 5 }).WithCompareOrder().IsNot([6, 5, 7, 1]);
        exception = Assert.Throws<AssertThatException>(() =>
            AssertThat.TestValue(new[] { 6, 7, 1, 5 }).WithCompareOrder(false).IsNot([6, 5, 7, 1]));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are the same");
    }
}