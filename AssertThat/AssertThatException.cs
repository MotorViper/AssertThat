namespace AssertThatLibrary;

/// <summary>
///     Exception thrown by AssertThat objects.
/// </summary>
/// <param name="message">Message for caller.</param>
public class AssertThatException(string message) : Exception(message);