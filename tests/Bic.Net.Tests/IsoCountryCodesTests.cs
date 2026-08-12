using Bic;

namespace Bic.Net.Tests;

public class IsoCountryCodesTests
{
    private const int OfficiallyAssignedAlpha2CodeCount = 249;

    [Fact]
    public void All_contains_exactly_the_officially_assigned_alpha2_code_count()
    {
        Assert.Equal(OfficiallyAssignedAlpha2CodeCount, IsoCountryCodes.All.Count);
    }

    [Theory]
    [InlineData("DE")] // Germany
    [InlineData("GB")] // United Kingdom
    [InlineData("US")] // United States
    [InlineData("ZA")] // South Africa
    [InlineData("NG")] // Nigeria
    [InlineData("JP")] // Japan
    [InlineData("BR")] // Brazil
    [InlineData("AU")] // Australia
    [InlineData("CA")] // Canada
    [InlineData("IN")] // India
    [InlineData("CN")] // China
    [InlineData("FR")] // France
    [InlineData("CH")] // Switzerland
    [InlineData("SG")] // Singapore
    [InlineData("AE")] // United Arab Emirates
    [InlineData("SS")] // South Sudan (assigned 2011)
    [InlineData("TL")] // Timor-Leste
    [InlineData("MF")] // Saint Martin (French part)
    [InlineData("BQ")] // Bonaire, Sint Eustatius and Saba
    public void Contains_recognizes_assigned_codes(string alpha2)
    {
        Assert.True(IsoCountryCodes.Contains(alpha2));
    }

    [Theory]
    [InlineData("UK")] // common mistake; the assigned code for the United Kingdom is "GB"
    [InlineData("ZZ")] // user-assigned range, never officially allocated
    [InlineData("QQ")] // user-assigned range, never officially allocated
    [InlineData("AA")] // user-assigned range, never officially allocated
    [InlineData("AN")] // Netherlands Antilles, withdrawn from ISO 3166-1 in 2010
    [InlineData("CS")] // Serbia and Montenegro, withdrawn from ISO 3166-1 in 2006
    [InlineData("EU")] // supranational code used by SWIFT/EU, not part of ISO 3166-1
    [InlineData("XK")] // Kosovo; used informally by some organizations, not officially assigned
    [InlineData("")]
    [InlineData("gb")] // lower case; Contains is ordinal and case-sensitive by design
    public void Contains_rejects_unassigned_or_incorrectly_cased_codes(string alpha2)
    {
        Assert.False(IsoCountryCodes.Contains(alpha2));
    }

    [Fact]
    public void All_contains_only_upper_case_two_letter_codes()
    {
        Assert.All(IsoCountryCodes.All, code =>
        {
            Assert.Equal(2, code.Length);
            Assert.Equal(code.ToUpperInvariant(), code);
        });
    }
}
