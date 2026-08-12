namespace Bic;

/// <summary>
/// Thrown when a string does not conform to the SWIFT BIC (ISO 9362) structure: wrong length,
/// an invalid character in a segment, or a country code that is not a recognized ISO 3166-1
/// alpha-2 code.
/// </summary>
public sealed class BicFormatException : FormatException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BicFormatException"/> class with a message
    /// describing why the input could not be parsed as a BIC.
    /// </summary>
    /// <param name="message">A message describing the validation failure.</param>
    public BicFormatException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BicFormatException"/> class with a message
    /// describing why the input could not be parsed as a BIC and a reference to the inner
    /// exception that caused it.
    /// </summary>
    /// <param name="message">A message describing the validation failure.</param>
    /// <param name="innerException">The exception that is the cause of this exception.</param>
    public BicFormatException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
