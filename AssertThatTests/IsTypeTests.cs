using AssertThatLibrary;
using AssertThatTests.TestClasses;

namespace AssertThatTests;

public class IsTypeTests
{
    [Fact]
    public void TestSpotsDifferent()
    {
        object a = 1;
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(a).Is<double>());
        AssertThat.TestValue(exception.Message).Is("Test value type [Int32] is not of type [Double]");
    }

    [Fact]
    public void TestSpotsDifferentType()
    {
        var a = 1;
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(a).Is<double>());
        AssertThat.TestValue(exception.Message).Is("Test value type [Int32] is not of type [Double]");
    }

    [Fact]
    public void TestSpotsObjectFromInterface()
    {
        IAClass a = new AClass();
        AssertThat.TestValue(a).Is<AClass>();
        AssertThat.TestValue(a).WithExact().IsNot<AClass>();
    }

    [Fact]
    public void TestSpotsObjectFromInheritance()
    {
        AClass a = new CClass();
        AssertThat.TestValue(a).Is<CClass>();
        AssertThat.TestValue(a).WithExact().IsNot<CClass>();
    }

    [Fact]
    public void TestSpotsObjectFromChild()
    {
        var a = new AClass();
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(a).Is<CClass>());
        AssertThat.TestValue(exception.Message).Is("Test value type [AClass] is not of type [CClass]");
    }

    [Fact]
    public void TestSpotsSame()
    {
        object a = 1;
        AssertThat.TestValue(a).Is<int>();
        AssertThat.TestValue(a).WithExact().IsNot<int>();
    }

    [Fact]
    public void TestSpotsSameType()
    {
        var a = 1;
        AssertThat.TestValue(a).Is<int>();
        AssertThat.TestValue(a).WithExact().Is<int>();
    }

    [Fact]
    public void TestSpotsNull()
    {
        object? a = null;
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(a).Is<int>());
        AssertThat.TestValue(exception.Message).Is("Test value is null so cannot check type");
    }
}