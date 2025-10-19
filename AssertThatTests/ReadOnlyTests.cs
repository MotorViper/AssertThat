using AssertThatLibrary;
using AssertThatTests.TestClasses;

namespace AssertThatTests;

public class ReadOnlyTests
{
    [Fact]
    public void TestCheckReadOnlySuccess()
    {
        var a = new AClass { D = 1, L = 1, S = 3 };
        var b = new DClass { D = 1, L = 1 };
        AssertThat.TestValue(b).WithCheckReadOnly("S", false).IsLike(a);
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(b).WithCheckReadOnly("S").IsLike(a));
        AssertThat.TestValue(exception.Message).Is("Actual and expected values are different: S = [5] != [3]");
    }
}