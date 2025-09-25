using AssertThatLibrary;

namespace AssertThatTests;

public class IsInTests
{
    [Fact]
    public void TestMatchesInts()
    {
        AssertThat.TestValue(1).IsIn(6, 7, 1, 5);
    }

    [Fact]
    public void TestNotMatchesInts()
    {
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(3).IsIn(6, 7, 1, 5));
        AssertThat.TestValue(exception.Message).Is("[3] is not in collection");
    }
}