namespace AssertThatTests.TestClasses;

public record DClass
{
    public long L { get; init; }
    public decimal D { get; init; }
    public short S { get; } = 5;
}