using Bic;

namespace Bic.Net.Tests;

public class BicFormatTests
{
    [Fact]
    public void Segment_lengths_match_ISO_9362()
    {
        Assert.Equal(4, BicFormat.InstitutionCodeLength);
        Assert.Equal(2, BicFormat.CountryCodeLength);
        Assert.Equal(2, BicFormat.LocationCodeLength);
        Assert.Equal(3, BicFormat.BranchCodeLength);
    }

    [Fact]
    public void Total_lengths_are_eight_and_eleven()
    {
        Assert.Equal(8, BicFormat.HeadOfficeLength);
        Assert.Equal(11, BicFormat.BranchLength);
    }

    [Fact]
    public void Segment_offsets_are_contiguous_and_in_order()
    {
        Assert.Equal(0, BicFormat.InstitutionCodeOffset);
        Assert.Equal(4, BicFormat.CountryCodeOffset);
        Assert.Equal(6, BicFormat.LocationCodeOffset);
        Assert.Equal(8, BicFormat.BranchCodeOffset);
    }

    [Fact]
    public void Test_indicator_is_the_digit_zero()
    {
        Assert.Equal('0', BicFormat.TestBicIndicator);
    }

    [Fact]
    public void Primary_office_branch_code_is_XXX()
    {
        Assert.Equal("XXX", BicFormat.PrimaryOfficeBranchCode);
    }
}
