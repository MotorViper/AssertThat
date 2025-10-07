using AssertThatLibrary;
using AssertThatTests.TestClasses;

// ReSharper disable ConvertToConstant.Local

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
    public void TestSpotsObjectFromInterface()
    {
        IAClass a = new AClass();
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(a).IsNot<AClass>());
        AssertThat.TestValue(exception.Message).Is("Test value is of type [AClass]");
        AssertThat.TestValue(a).WithExact().IsNot<AClass>();
    }

    [Fact]
    public void TestSpotsObjectFromInheritance()
    {
        AClass a = new CClass();
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(a).IsNot<CClass>());
        AssertThat.TestValue(exception.Message).Is("Test value is of type [CClass]");
        AssertThat.TestValue(a).WithExact().IsNot<CClass>();
    }

    [Fact]
    public void TestSpotsObjectFromChild()
    {
        var a = new AClass();
        AssertThat.TestValue(a).IsNot<CClass>();
        AssertThat.TestValue(a).WithExact().IsNot<CClass>();
    }

    [Fact]
    public void TestSpotsSame()
    {
        object a = 1;
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(a).IsNot<int>());
        AssertThat.TestValue(exception.Message).Is("Test value is of type [Int32]");
        AssertThat.TestValue(a).WithExact().IsNot<int>();
    }

    [Fact]
    public void TestSpotsSameType()
    {
        var a = 1;
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(a).IsNot<int>());
        AssertThat.TestValue(exception.Message).Is("Test value is of type [Int32]");
        AssertThat.TestValue(a).WithExact().Is<int>();
    }

    [Fact]
    public void TestSpotsNull()
    {
        object? a = null;
        var exception = Assert.Throws<AssertThatException>(() => AssertThat.TestValue(a).IsNot<int>());
        AssertThat.TestValue(exception.Message).Is("Test value is null so cannot check type");
    }
}