using System.Diagnostics.CodeAnalysis;

namespace Bic;

/// <summary>
/// Validates and parses SWIFT BIC (ISO 9362) business identifier codes: an 8 or 11 character code
/// comprising a 4-letter institution code, a 2-letter ISO 3166-1 alpha-2 country code, a
/// 2-character location code, and an optional 3-character branch code.
/// </summary>
public static class BicParser
{
    /// <summary>
    /// Determines whether <paramref name="value"/> is a structurally valid BIC: the correct
    /// length, letters-only institution and country segments, an alphanumeric location segment, an
    /// alphanumeric branch segment when present, and a country segment that is a recognized
    /// ISO 3166-1 alpha-2 code. Comparison is case-insensitive; a lower-case or mixed-case BIC is
    /// considered valid.
    /// </summary>
    /// <param name="value">The candidate BIC.</param>
    /// <returns><see langword="true"/> when <paramref name="value"/> is a valid BIC; otherwise <see langword="false"/>.</returns>
    public static bool IsValid([NotNullWhen(true)] string? value) => TryParse(value, out _);

    /// <summary>
    /// Parses <paramref name="value"/> into a <see cref="BicCode"/>.
    /// </summary>
    /// <param name="value">The BIC to parse.</param>
    /// <returns>The parsed, normalized <see cref="BicCode"/>.</returns>
    /// <exception cref="BicFormatException">
    /// <paramref name="value"/> is not a structurally valid BIC: it is null, empty, of a length
    /// other than <see cref="BicFormat.HeadOfficeLength"/> or <see cref="BicFormat.BranchLength"/>,
    /// contains a character outside the allowed class for its segment, or its country segment is
    /// not a recognized ISO 3166-1 alpha-2 code.
    /// </exception>
    public static BicCode Parse(string value)
    {
        var failure = TryParseCore(value, out var result);
        if (failure != BicValidationFailure.None)
        {
            throw new BicFormatException(DescribeFailure(value, failure));
        }

        return result!;
    }

    /// <summary>
    /// Attempts to parse <paramref name="value"/> into a <see cref="BicCode"/>, without throwing
    /// when it is not a valid BIC.
    /// </summary>
    /// <param name="value">The candidate BIC.</param>
    /// <param name="result">
    /// When this method returns <see langword="true"/>, the parsed, normalized
    /// <see cref="BicCode"/>; otherwise <see langword="null"/>.
    /// </param>
    /// <returns><see langword="true"/> when <paramref name="value"/> is a valid BIC; otherwise <see langword="false"/>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? value, out BicCode? result)
    {
        var failure = TryParseCore(value, out result);
        return failure == BicValidationFailure.None;
    }

    private static BicValidationFailure TryParseCore(string? value, out BicCode? result)
    {
        result = null;

        if (string.IsNullOrEmpty(value))
        {
            return BicValidationFailure.NullOrEmpty;
        }

        if (value.Length != BicFormat.HeadOfficeLength && value.Length != BicFormat.BranchLength)
        {
            return BicValidationFailure.InvalidLength;
        }

        Span<char> normalized = stackalloc char[value.Length];
        for (var i = 0; i < value.Length; i++)
        {
            normalized[i] = char.ToUpperInvariant(value[i]);
        }

        var institution = normalized.Slice(BicFormat.InstitutionCodeOffset, BicFormat.InstitutionCodeLength);
        if (!IsAllAsciiLetters(institution))
        {
            return BicValidationFailure.InvalidInstitutionCode;
        }

        var country = normalized.Slice(BicFormat.CountryCodeOffset, BicFormat.CountryCodeLength);
        if (!IsAllAsciiLetters(country))
        {
            return BicValidationFailure.InvalidCountryCode;
        }

        var countryText = country.ToString();
        if (!IsoCountryCodes.Contains(countryText))
        {
            return BicValidationFailure.UnrecognizedCountryCode;
        }

        var location = normalized.Slice(BicFormat.LocationCodeOffset, BicFormat.LocationCodeLength);
        if (!IsAllAsciiLettersOrDigits(location))
        {
            return BicValidationFailure.InvalidLocationCode;
        }

        var branch = string.Empty;
        if (value.Length == BicFormat.BranchLength)
        {
            var branchSpan = normalized.Slice(BicFormat.BranchCodeOffset, BicFormat.BranchCodeLength);
            if (!IsAllAsciiLettersOrDigits(branchSpan))
            {
                return BicValidationFailure.InvalidBranchCode;
            }

            branch = branchSpan.ToString();
        }

        result = new BicCode
        {
            Institution = institution.ToString(),
            Country = countryText,
            Location = location.ToString(),
            Branch = branch,
        };

        return BicValidationFailure.None;
    }

    private static bool IsAllAsciiLetters(ReadOnlySpan<char> segment)
    {
        foreach (var c in segment)
        {
            if (!char.IsAsciiLetterUpper(c))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsAllAsciiLettersOrDigits(ReadOnlySpan<char> segment)
    {
        foreach (var c in segment)
        {
            if (!char.IsAsciiLetterUpper(c) && !char.IsAsciiDigit(c))
            {
                return false;
            }
        }

        return true;
    }

    private static string DescribeFailure(string? value, BicValidationFailure failure) => failure switch
    {
        BicValidationFailure.NullOrEmpty =>
            "BIC must not be null or empty.",
        BicValidationFailure.InvalidLength =>
            $"BIC must be {BicFormat.HeadOfficeLength} or {BicFormat.BranchLength} characters long, but \"{value}\" is {value!.Length} characters long.",
        BicValidationFailure.InvalidInstitutionCode =>
            $"The first {BicFormat.InstitutionCodeLength} characters of a BIC (the institution code) must be letters, but \"{value}\" has a non-letter there.",
        BicValidationFailure.InvalidCountryCode =>
            $"Characters {BicFormat.CountryCodeOffset + 1} and {BicFormat.CountryCodeOffset + BicFormat.CountryCodeLength} of a BIC (the country code) must be letters, but \"{value}\" has a non-letter there.",
        BicValidationFailure.UnrecognizedCountryCode =>
            $"\"{value}\" does not contain a recognized ISO 3166-1 alpha-2 country code.",
        BicValidationFailure.InvalidLocationCode =>
            $"Characters {BicFormat.LocationCodeOffset + 1} and {BicFormat.LocationCodeOffset + BicFormat.LocationCodeLength} of a BIC (the location code) must be letters or digits, but \"{value}\" has an invalid character there.",
        BicValidationFailure.InvalidBranchCode =>
            $"The last {BicFormat.BranchCodeLength} characters of an 11-character BIC (the branch code) must be letters or digits, but \"{value}\" has an invalid character there.",
        _ => $"\"{value}\" is not a valid BIC.",
    };
}
