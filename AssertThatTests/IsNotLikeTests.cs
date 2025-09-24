using AssertThatLibrary;
using AssertThatTests.TestClasses;

namespace AssertThatTests;

public class IsNotLikeTests
{
    [Fact]
    public void TestMatchesInts()
    {
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(1).IsNotLike((short)1));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are the same");
    }

    [Fact]
    public void TestMatchesBools()
    {
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(false).IsNotLike("False"));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are the same");
    }

    [Fact]
    public void TestMatchesStrings()
    {
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue("1").IsNotLike(1));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are the same");
    }

    [Fact]
    public void TestMatchesDateTimes()
    {
        var value = TimeSpan.Zero;
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(value)
            .WithIgnoredProperties("W:Day*", "Minutes", "Seconds", "W:Nanosecond*", "W:Total*", "Date",
                "Kind", "Minute", "Month", "Second", "TimeOfDay", "Year")
            .WithEquivalentProperty("Microseconds", "Microsecond")
            .WithEquivalentProperties(("Hours", "Hour"),
                ("Milliseconds", "Millisecond"))
            .IsNotLike(new DateTime()));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are the same");
    }

    [Fact]
    public void TestMatchesDateTimesNotFullyConverted()
    {
        var value = TimeSpan.Zero;
        AssertThat.TestValue(value)
            .WithIgnoredProperties("W:Day*", "Minutes", "Seconds", "W:Nanosecond*", "W:Total*", "Date",
                "Kind", "Minute", "Month", "Second", "TimeOfDay", "Year")
            .WithEquivalentProperties(("Hours", "Hour"),
                ("Milliseconds", "Millisecond"))
            .IsNotLike(new DateTime());
    }

    [Fact]
    public void TestMatchesClass()
    {
        var value = new AClass { S = 4, L = 2, D = 3 };
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(value).IsNotLike(new BClass { S = 4, L = 2, D = 3 }));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are the same");
    }
}