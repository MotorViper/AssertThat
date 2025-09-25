using AssertThatLibrary;
using AssertThatTests.TestClasses;

namespace AssertThatTests;

public class IsLikeTests
{
    [Fact]
    public void TestMatchesInts()
    {
        AssertThat.TestValue(1).IsLike((short)1);
    }

    [Fact]
    public void TestMatchesBools()
    {
        AssertThat.TestValue(false).IsLike("False");
    }

    [Fact]
    public void TestMatchesStrings()
    {
        AssertThat.TestValue("1").IsLike(1);
    }

    [Fact]
    public void TestMatchesDateTimes()
    {
        var value = TimeSpan.Zero;
        AssertThat.TestValue(value)
            .WithIgnoredProperties("W:*Da*", "W:Total*", "Kind", "Month", "Year")
            .WithEquivalentProperty<DateTime>(x => x.Hours, x => x.Hour)
            .WithEquivalentProperties(("Hours", "Hour"),
                ("Minutes", "Minute"),
                ("Seconds", "Second"),
                ("Nanoseconds", "Nanosecond"),
                ("Microseconds", "Microsecond"),
                ("Milliseconds", "Millisecond"))
            .IsLike(new DateTime());
    }

    [Fact]
    public void TestMatchesDateTimesNotFullyConverted()
    {
        var value = TimeSpan.Zero;
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(value)
            .WithIgnoredProperties("W:Day*", "Minutes", "Seconds", "W:Nanosecond*", "W:Total*", "Date",
                "Kind", "Minute", "Month", "Second", "TimeOfDay", "Year")
            .WithEquivalentProperties(("Hours", "Hour"),
                ("Milliseconds", "Millisecond"))
            .IsLike(new DateTime()));
        AssertThat.TestValue(exception.Message).Is("Microseconds: Microseconds is not in DateTime");
    }

    [Fact]
    public void TestMatchesClass()
    {
        var value = new AClass { S = 4, L = 2, D = 3 };
        AssertThat.TestValue(value).IsLike(new BClass { S = 4, L = 2, D = 3 });
    }
}