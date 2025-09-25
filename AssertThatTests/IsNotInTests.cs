using AssertThatLibrary;

namespace AssertThatTests;

public class IsNotInTests
{
    [Fact]
    public void TestMatchesInts()
    {
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(1).IsNotIn(6, 7, 1, 5));
        AssertThat.TestValue(exception.Message).Is("[1] is in collection");
    }

    [Fact]
    public void TestNotMatchesInts()
    {
        AssertThat.TestValue(3).IsNotIn(6, 7, 1, 5);
    }
}