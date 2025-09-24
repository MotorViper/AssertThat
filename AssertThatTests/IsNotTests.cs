using AssertThatLibrary;
using AssertThatTests.TestClasses;

namespace AssertThatTests;

public class IsNotTests
{
    [Fact]
    public void TestMatchesInts()
    {
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(1).IsNot(1));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are the same");
    }

    [Fact]
    public void TestMatchesBools()
    {
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(false).IsNot(false));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are the same");
    }

    [Fact]
    public void TestMatchesStrings()
    {
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue("1").IsNot("1"));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are the same");
    }

    [Fact]
    public void TestMatchesDateTimes()
    {
        var value = new DateTime();
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(value).IsNot(value));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are the same");
    }

    [Fact]
    public void TestMatchesClass()
    {
        var value = new AClass { S = 4, L = 2, D = 3 };
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(value).IsNot(value));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are the same");
    }

    [Fact]
    public void TestMismatchedValues()
    {
        AssertThat.TestValue(1).IsNot(2);
    }

    [Fact]
    public void TestMismatchedBoolValues()
    {
        AssertThat.TestValue(false).IsNot(true);
    }

    [Fact]
    public void TestMismatchedDateTimes()
    {
        var value = new DateTime();
        AssertThat.TestValue(value).IsNot(value.AddDays(1));
    }

    [Fact]
    public void TestMismatchedNullValue()
    {
        AssertThat.TestValue(new AClass()).IsNot(null);
    }

    [Fact]
    public void TestMismatchedClassValue()
    {
        var value = new AClass { S = 4, L = 2, D = 3 };
        AssertThat.TestValue(value).IsNot(value with { S = 10 });
    }
}