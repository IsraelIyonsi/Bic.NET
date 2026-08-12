using System.Collections.Frozen;

namespace Bic;

/// <summary>
/// The set of officially assigned ISO 3166-1 alpha-2 country codes, used to validate the country
/// segment of a BIC. The set reflects the codes assigned at the time of this library's release;
/// ISO occasionally assigns new codes or withdraws old ones (as happened historically with "AN"
/// and "CS"), so treat it as current as of release rather than a live feed from the ISO 3166
/// Maintenance Agency.
/// </summary>
public static class IsoCountryCodes
{
    private static readonly FrozenSet<string> Codes = new[]
    {
        // A
        "AD", "AE", "AF", "AG", "AI", "AL", "AM", "AO", "AQ", "AR", "AS", "AT", "AU", "AW", "AX", "AZ",
        // B
        "BA", "BB", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BL", "BM", "BN", "BO", "BQ", "BR", "BS",
        "BT", "BV", "BW", "BY", "BZ",
        // C
        "CA", "CC", "CD", "CF", "CG", "CH", "CI", "CK", "CL", "CM", "CN", "CO", "CR", "CU", "CV", "CW",
        "CX", "CY", "CZ",
        // D
        "DE", "DJ", "DK", "DM", "DO", "DZ",
        // E
        "EC", "EE", "EG", "EH", "ER", "ES", "ET",
        // F
        "FI", "FJ", "FK", "FM", "FO", "FR",
        // G
        "GA", "GB", "GD", "GE", "GF", "GG", "GH", "GI", "GL", "GM", "GN", "GP", "GQ", "GR", "GS", "GT",
        "GU", "GW", "GY",
        // H
        "HK", "HM", "HN", "HR", "HT", "HU",
        // I
        "ID", "IE", "IL", "IM", "IN", "IO", "IQ", "IR", "IS", "IT",
        // J
        "JE", "JM", "JO", "JP",
        // K
        "KE", "KG", "KH", "KI", "KM", "KN", "KP", "KR", "KW", "KY", "KZ",
        // L
        "LA", "LB", "LC", "LI", "LK", "LR", "LS", "LT", "LU", "LV", "LY",
        // M
        "MA", "MC", "MD", "ME", "MF", "MG", "MH", "MK", "ML", "MM", "MN", "MO", "MP", "MQ", "MR", "MS",
        "MT", "MU", "MV", "MW", "MX", "MY", "MZ",
        // N
        "NA", "NC", "NE", "NF", "NG", "NI", "NL", "NO", "NP", "NR", "NU", "NZ",
        // O
        "OM",
        // P
        "PA", "PE", "PF", "PG", "PH", "PK", "PL", "PM", "PN", "PR", "PS", "PT", "PW", "PY",
        // Q
        "QA",
        // R
        "RE", "RO", "RS", "RU", "RW",
        // S
        "SA", "SB", "SC", "SD", "SE", "SG", "SH", "SI", "SJ", "SK", "SL", "SM", "SN", "SO", "SR", "SS",
        "ST", "SV", "SX", "SY", "SZ",
        // T
        "TC", "TD", "TF", "TG", "TH", "TJ", "TK", "TL", "TM", "TN", "TO", "TR", "TT", "TV", "TW", "TZ",
        // U
        "UA", "UG", "UM", "US", "UY", "UZ",
        // V
        "VA", "VC", "VE", "VG", "VI", "VN", "VU",
        // W
        "WF", "WS",
        // Y
        "YE", "YT",
        // Z
        "ZA", "ZM", "ZW",
    }.ToFrozenSet(StringComparer.Ordinal);

    /// <summary>
    /// All officially assigned ISO 3166-1 alpha-2 country codes recognized by this library, as
    /// upper-case two-letter strings.
    /// </summary>
    public static IReadOnlySet<string> All => Codes;

    /// <summary>
    /// Determines whether <paramref name="alpha2"/> is a recognized, officially assigned
    /// ISO 3166-1 alpha-2 country code. The comparison is ordinal and case-sensitive: callers
    /// should upper-case the input first.
    /// </summary>
    /// <param name="alpha2">The candidate two-letter country code.</param>
    /// <returns><see langword="true"/> when the code is recognized; otherwise <see langword="false"/>.</returns>
    public static bool Contains(string alpha2) => Codes.Contains(alpha2);
}
