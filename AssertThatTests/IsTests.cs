using AssertThatLibrary;
using AssertThatTests.TestClasses;

namespace AssertThatTests;

public class IsTests
{
    [Fact]
    public void TestMatchesInts()
    {
        AssertThat.TestValue(1).Is(1);
    }

    [Fact]
    public void TestMatchesBools()
    {
        AssertThat.TestValue(false).Is(false);
    }

    [Fact]
    public void TestMatchesStrings()
    {
        AssertThat.TestValue("1").Is("1");
    }

    [Fact]
    public void TestMatchesDateTimes()
    {
        var value = new DateTime();
        AssertThat.TestValue(value).Is(value);
    }

    [Fact]
    public void TestMatchesClass()
    {
        var value = new AClass { S = 4, L = 2, D = 3 };
        AssertThat.TestValue(value).Is(value);
    }

    [Fact]
    public void TestMismatchedValues()
    {
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(1).Is(2));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are different: [1] != [2]");
    }

    [Fact]
    public void TestMismatchedBoolValues()
    {
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(false).Is(true));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are different: [False] != [True]");
    }

    [Fact]
    public void TestMismatchedDateTimes()
    {
        var value = new DateTime();
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(value).Is(value.AddDays(1)));
        AssertThat.TestValue(exception.Message)
            .Is("Actual and expected values are different: [01/01/0001 00:00:00] != [02/01/0001 00:00:00]");
    }

    [Fact]
    public void TestMismatchedNullValue()
    {
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(new AClass()).Is(null));
        AssertThat.TestValue(exception.Message)
            .Is("Actual and expected values are different: [AClass { S = 0, L = 0, D = 0 }] != [null]");
    }

    [Fact]
    public void TestMismatchedClassValue()
    {
        var value = new AClass { S = 4, L = 2, D = 3 };
        var exception =
            Assert.Throws<AssertThatException>(() => AssertThat.TestValue(value).Is(value with { S = 10 }));
        AssertThat.TestValue(exception.Message)
            .Is(
                "Actual and expected values are different: [AClass { S = 4, L = 2, D = 3 }] != [AClass { S = 10, L = 2, D = 3 }]");
    }
}