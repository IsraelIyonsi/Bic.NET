using Bic;

namespace Bic.Net.Tests;

public class BicParserValidBicTests
{
    public static TheoryData<string, string, string, string, string, bool, bool> WellFormedBics()
    {
        var data = new TheoryData<string, string, string, string, string, bool, bool>();

        // DEUTDEFF / DEUTDEFF500 is the canonical ISO 9362 worked example: Deutsche Bank AG's
        // primary office in Frankfurt am Main, and its branch 500. This pairing is the example
        // used in the Wikipedia ISO 9362 article and reused as a reference fixture across BIC
        // validation libraries in multiple languages.
        data.Add("DEUTDEFF", "DEUT", "DE", "FF", "", false, true);
        data.Add("DEUTDEFF500", "DEUT", "DE", "FF", "500", false, false);
        data.Add("DEUTDEFFXXX", "DEUT", "DE", "FF", "XXX", false, true);

        // Widely published example BICs reused across public BIC-validation library test suites
        // (this project does not have live SWIFT BIC directory access to re-verify registration).
        data.Add("BARCGB22", "BARC", "GB", "22", "", false, true); // Barclays Bank Plc, London
        data.Add("CHASUS33", "CHAS", "US", "33", "", false, true); // JPMorgan Chase Bank, New York
        data.Add("CHASUS33XXX", "CHAS", "US", "33", "XXX", false, true);
        data.Add("NEDSZAJJ", "NEDS", "ZA", "JJ", "", false, true); // Nedbank Ltd, Johannesburg

        // Lower-case and mixed-case input normalizes to upper case.
        data.Add("deutdeff", "DEUT", "DE", "FF", "", false, true);
        data.Add("DeutDeFf500", "DEUT", "DE", "FF", "500", false, false);

        // Location code may be entirely digits (BARCGB22 above already covers this), entirely
        // letters, or mixed; branch code may be letters, digits, or a mix.
        data.Add("AAAAGBAA", "AAAA", "GB", "AA", "", false, true);
        data.Add("AAAAGB12", "AAAA", "GB", "12", "", false, true);
        data.Add("AAAAGB1A1AB", "AAAA", "GB", "1A", "1AB", false, false);

        // Second character of the location code equal to '0' marks a test BIC. These BICs are
        // constructed purely to exercise that rule; they are not claimed to be real, registered
        // institution codes.
        data.Add("TESTGBA0", "TEST", "GB", "A0", "", true, true);
        data.Add("TESTGBA0XXX", "TEST", "GB", "A0", "XXX", true, true);
        data.Add("TESTGBA0123", "TEST", "GB", "A0", "123", true, false);

        return data;
    }

    [Theory]
    [MemberData(nameof(WellFormedBics))]
    public void IsValid_returns_true_for_well_formed_bics(
        string input, string institution, string country, string location, string branch, bool isTestBic, bool isPrimaryOffice)
    {
        _ = institution;
        _ = country;
        _ = location;
        _ = branch;
        _ = isTestBic;
        _ = isPrimaryOffice;

        Assert.True(BicParser.IsValid(input));
    }

    [Theory]
    [MemberData(nameof(WellFormedBics))]
    public void Parse_extracts_exact_segments(
        string input, string institution, string country, string location, string branch, bool isTestBic, bool isPrimaryOffice)
    {
        var bic = BicParser.Parse(input);

        Assert.Equal(institution, bic.Institution);
        Assert.Equal(country, bic.Country);
        Assert.Equal(location, bic.Location);
        Assert.Equal(branch, bic.Branch);
        Assert.Equal(isTestBic, bic.IsTestBic);
        Assert.Equal(isPrimaryOffice, bic.IsPrimaryOffice);
    }

    [Theory]
    [MemberData(nameof(WellFormedBics))]
    public void TryParse_returns_true_and_matches_parse(
        string input, string institution, string country, string location, string branch, bool isTestBic, bool isPrimaryOffice)
    {
        _ = institution;
        _ = country;
        _ = location;
        _ = branch;
        _ = isTestBic;
        _ = isPrimaryOffice;

        var parsed = BicParser.Parse(input);

        Assert.True(BicParser.TryParse(input, out var tried));
        Assert.Equal(parsed, tried);
    }

    [Fact]
    public void Parse_normalizes_head_office_and_explicit_primary_office_to_the_same_canonical_value()
    {
        var headOffice = BicParser.Parse("deutdeff");
        var explicitPrimaryOffice = BicParser.Parse("DEUTDEFFXXX");

        Assert.Equal("DEUTDEFFXXX", headOffice.CanonicalValue);
        Assert.Equal(headOffice.CanonicalValue, explicitPrimaryOffice.CanonicalValue);
        Assert.Equal("DEUTDEFF", headOffice.Value);
        Assert.Equal("DEUTDEFFXXX", explicitPrimaryOffice.Value);
    }
}
