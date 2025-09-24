namespace AssertThatLibrary;

public static class AssertThat
{
    public static AssertThatChecker<TActual> TestValue<TActual>(TActual item)
    {
        return new AssertThatChecker<TActual>(item);
    }
}
