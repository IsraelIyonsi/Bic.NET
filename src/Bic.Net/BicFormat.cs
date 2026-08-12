namespace Bic;

/// <summary>
/// Structural constants defining the layout of a SWIFT BIC (ISO 9362) as specified by the standard:
/// a 4-letter institution code, a 2-letter ISO 3166-1 alpha-2 country code, a 2-character location
/// code, and an optional 3-character branch code.
/// </summary>
public static class BicFormat
{
    /// <summary>
    /// Length, in characters, of the institution (bank) code segment.
    /// </summary>
    public const int InstitutionCodeLength = 4;

    /// <summary>
    /// Length, in characters, of the ISO 3166-1 alpha-2 country code segment.
    /// </summary>
    public const int CountryCodeLength = 2;

    /// <summary>
    /// Length, in characters, of the location code segment.
    /// </summary>
    public const int LocationCodeLength = 2;

    /// <summary>
    /// Length, in characters, of the optional branch code segment.
    /// </summary>
    public const int BranchCodeLength = 3;

    /// <summary>
    /// Zero-based offset, within a BIC, at which the institution code segment starts.
    /// </summary>
    public const int InstitutionCodeOffset = 0;

    /// <summary>
    /// Zero-based offset, within a BIC, at which the country code segment starts.
    /// </summary>
    public const int CountryCodeOffset = InstitutionCodeOffset + InstitutionCodeLength;

    /// <summary>
    /// Zero-based offset, within a BIC, at which the location code segment starts.
    /// </summary>
    public const int LocationCodeOffset = CountryCodeOffset + CountryCodeLength;

    /// <summary>
    /// Zero-based offset, within a BIC, at which the branch code segment starts.
    /// </summary>
    public const int BranchCodeOffset = LocationCodeOffset + LocationCodeLength;

    /// <summary>
    /// Zero-based offset, within the location code segment, of the character that indicates a test BIC.
    /// </summary>
    public const int TestIndicatorOffsetInLocationCode = 1;

    /// <summary>
    /// Total length, in characters, of a head office BIC that omits the branch code segment.
    /// </summary>
    public const int HeadOfficeLength = InstitutionCodeLength + CountryCodeLength + LocationCodeLength;

    /// <summary>
    /// Total length, in characters, of a BIC that includes the branch code segment.
    /// </summary>
    public const int BranchLength = HeadOfficeLength + BranchCodeLength;

    /// <summary>
    /// The character that, when it appears as the second character of the location code, marks
    /// the BIC as a test BIC rather than a live, production BIC.
    /// </summary>
    public const char TestBicIndicator = '0';

    /// <summary>
    /// The conventional branch code that denotes an institution's primary office rather than a
    /// specific branch.
    /// </summary>
    public const string PrimaryOfficeBranchCode = "XXX";
}
