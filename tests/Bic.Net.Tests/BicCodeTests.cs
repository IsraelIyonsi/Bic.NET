using Bic;

namespace Bic.Net.Tests;

public class BicCodeTests
{
    [Theory]
    [InlineData("", true)]
    [InlineData("XXX", true)]
    [InlineData("500", false)]
    [InlineData("001", false)]
    public void IsPrimaryOffice_is_true_only_for_empty_or_XXX_branch(string branch, bool expected)
    {
        var bic = new BicCode
        {
            Institution = "DEUT",
            Country = "DE",
            Location = "FF",
            Branch = branch,
        };

        Assert.Equal(expected, bic.IsPrimaryOffice);
    }

    [Fact]
    public void Value_preserves_the_source_length()
    {
        var headOffice = BicParser.Parse("DEUTDEFF");
        var branch = BicParser.Parse("DEUTDEFF500");

        Assert.Equal("DEUTDEFF", headOffice.Value);
        Assert.Equal(8, headOffice.Value.Length);
        Assert.Equal("DEUTDEFF500", branch.Value);
        Assert.Equal(11, branch.Value.Length);
    }

    [Fact]
    public void CanonicalValue_is_always_eleven_characters()
    {
        var headOffice = BicParser.Parse("DEUTDEFF");
        var branch = BicParser.Parse("DEUTDEFF500");
        var explicitPrimaryOffice = BicParser.Parse("DEUTDEFFXXX");

        Assert.Equal(11, headOffice.CanonicalValue.Length);
        Assert.Equal("DEUTDEFFXXX", headOffice.CanonicalValue);
        Assert.Equal("DEUTDEFF500", branch.CanonicalValue);
        Assert.Equal("DEUTDEFFXXX", explicitPrimaryOffice.CanonicalValue);
    }

    [Fact]
    public void ToString_returns_Value()
    {
        var bic = BicParser.Parse("DEUTDEFF500");

        Assert.Equal(bic.Value, bic.ToString());
    }

    [Fact]
    public void Equal_bics_parsed_from_equivalent_input_compare_equal()
    {
        var lowerCase = BicParser.Parse("deutdeff500");
        var upperCase = BicParser.Parse("DEUTDEFF500");

        Assert.Equal(upperCase, lowerCase);
        Assert.Equal(upperCase.GetHashCode(), lowerCase.GetHashCode());
    }

    [Fact]
    public void Head_office_and_explicit_XXX_branch_are_not_record_equal_despite_same_canonical_value()
    {
        var headOffice = BicParser.Parse("DEUTDEFF");
        var explicitPrimaryOffice = BicParser.Parse("DEUTDEFFXXX");

        Assert.NotEqual(headOffice, explicitPrimaryOffice);
        Assert.Equal(headOffice.CanonicalValue, explicitPrimaryOffice.CanonicalValue);
    }

    [Fact]
    public void Bics_with_different_segments_are_not_equal()
    {
        var deutsche = BicParser.Parse("DEUTDEFF");
        var barclays = BicParser.Parse("BARCGB22");

        Assert.NotEqual(deutsche, barclays);
    }
}
