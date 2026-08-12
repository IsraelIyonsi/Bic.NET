namespace Bic;

/// <summary>
/// Identifies why a candidate string failed BIC parsing. Internal to <see cref="BicParser"/>;
/// callers observe failures only through <see cref="BicFormatException"/>'s message or through
/// <see cref="BicParser.TryParse(string?, out BicCode?)"/> returning <see langword="false"/>.
/// </summary>
internal enum BicValidationFailure
{
    None,
    NullOrEmpty,
    InvalidLength,
    InvalidInstitutionCode,
    InvalidCountryCode,
    UnrecognizedCountryCode,
    InvalidLocationCode,
    InvalidBranchCode,
}
