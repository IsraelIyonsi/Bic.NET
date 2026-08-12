namespace Bic;

/// <summary>
/// A successfully parsed SWIFT BIC (ISO 9362), split into its institution, country, location and
/// branch segments. Instances are produced by <see cref="BicParser.Parse(string)"/> and
/// <see cref="BicParser.TryParse(string?, out BicCode?)"/>; all segments are upper-case, regardless
/// of the casing of the original input.
/// </summary>
public sealed record BicCode
{
    /// <summary>
    /// The 4-letter institution (bank) code.
    /// </summary>
    public required string Institution { get; init; }

    /// <summary>
    /// The 2-letter ISO 3166-1 alpha-2 country code.
    /// </summary>
    public required string Country { get; init; }

    /// <summary>
    /// The 2-character location code.
    /// </summary>
    public required string Location { get; init; }

    /// <summary>
    /// The 3-character branch code, or an empty string when the source BIC was an 8-character
    /// head office code that omitted the branch segment entirely.
    /// </summary>
    public required string Branch { get; init; }

    /// <summary>
    /// <see langword="true"/> when the second character of <see cref="Location"/> is
    /// <see cref="BicFormat.TestBicIndicator"/>, marking this as a test BIC rather than a live,
    /// production BIC. Computed from <see cref="Location"/>, so it can never contradict it.
    /// </summary>
    public bool IsTestBic => Location.Length > BicFormat.TestIndicatorOffsetInLocationCode
        && Location[BicFormat.TestIndicatorOffsetInLocationCode] == BicFormat.TestBicIndicator;

    /// <summary>
    /// <see langword="true"/> when this BIC identifies an institution's primary office: either the
    /// branch segment was omitted (an 8-character source BIC) or it is explicitly
    /// <see cref="BicFormat.PrimaryOfficeBranchCode"/>.
    /// </summary>
    public bool IsPrimaryOffice => Branch.Length == 0 || Branch == BicFormat.PrimaryOfficeBranchCode;

    /// <summary>
    /// The upper-case, normalized BIC, at the same length as the source input: 8 characters when
    /// the branch segment was omitted, 11 characters when it was present.
    /// </summary>
    public string Value => Institution + Country + Location + Branch;

    /// <summary>
    /// The upper-case, normalized BIC in its canonical 11-character form. An 8-character source
    /// BIC is treated as the head office: its missing branch segment is filled in with
    /// <see cref="BicFormat.PrimaryOfficeBranchCode"/>, so this value is suitable for comparing an
    /// 8-character head office BIC against its 11-character equivalent.
    /// </summary>
    public string CanonicalValue => Institution + Country + Location
        + (Branch.Length == 0 ? BicFormat.PrimaryOfficeBranchCode : Branch);

    /// <summary>
    /// Returns <see cref="Value"/>.
    /// </summary>
    /// <returns>The upper-case, normalized BIC at its source length.</returns>
    public override string ToString() => Value;
}
