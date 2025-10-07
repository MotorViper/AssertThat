namespace AssertThatTests.TestClasses;

public record AClass : IAClass
{
    public long L { get; init; }
    public decimal D { get; init; }
    public short S { get; init; }
}