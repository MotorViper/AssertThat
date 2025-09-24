using AssertThatLibrary;

namespace AssertThatTests;

public class IsNotTypeTests
{
    [Fact]
    public void TestSpotsDifferent()
    {
        object a = 1;
        AssertThat.TestValue(a).IsNot<double>();
    }

    [Fact]
    public void TestSpotsDifferentType()
    {
        var a = 1;
        AssertThat.TestValue(a).IsNot<double>();
    }

    [Fact]
    public void TestSpotsSame()
    {
        object a = 1;
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(a).IsNot<int>());
        AssertThat.TestValue(exception.Message).Is("Test value [1] is of type Int32");
    }

    [Fact]
    public void TestSpotsSameType()
    {
        var a = 1;
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(a).IsNot<int>());
        AssertThat.TestValue(exception.Message).Is("Test value [1] is of type Int32");
    }

    [Fact]
    public void TestSpotsNull()
    {
        object? a = null;
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(a).IsNot<int>());
        AssertThat.TestValue(exception.Message).Is("Test value is null so cannot check type");
    }
}