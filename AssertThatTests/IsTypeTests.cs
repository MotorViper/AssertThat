using AssertThatLibrary;

namespace AssertThatTests;

public class IsTypeTests
{
    [Fact]
    public void TestSpotsDifferent()
    {
        object a = 1;
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(a).Is<double>());
        AssertThat.TestValue(exception.Message).Is("Test value [1] is not of type Double");
    }

    [Fact]
    public void TestSpotsDifferentType()
    {
        var a = 1;
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(a).Is<double>());
        AssertThat.TestValue(exception.Message).Is("Test value [1] is not of type Double");
    }

    [Fact]
    public void TestSpotsSame()
    {
        object a = 1;
        AssertThat.TestValue(a).Is<int>();
    }

    [Fact]
    public void TestSpotsSameType()
    {
        var a = 1;
        AssertThat.TestValue(a).Is<int>();
    }

    [Fact]
    public void TestSpotsNull()
    {
        object? a = null;
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(a).Is<int>());
        AssertThat.TestValue(exception.Message).Is("Test value is null so cannot check type");
    }
}