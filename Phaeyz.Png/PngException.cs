namespace Phaeyz.Png;

/// <summary>
/// This exception is thrown if there is a serialization or deserialization issue.
/// </summary>
/// <param name="message">
/// Message details regarding the exception.
/// </param>
public class PngException(string message) : Exception(message) { }