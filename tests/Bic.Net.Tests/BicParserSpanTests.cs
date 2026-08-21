using Bic;

namespace Bic.Net.Tests;

public class BicParserSpanTests
{
    [Theory]
    [MemberData(nameof(BicParserValidBicTests.WellFormedBics), MemberType = typeof(BicParserValidBicTests))]
    public void IsValid_span_returns_true_for_well_formed_bics(
        string input, string institution, string country, string location, string branch, bool isTestBic, bool isPrimaryOffice)
    {
        _ = institution;
        _ = country;
        _ = location;
        _ = branch;
        _ = isTestBic;
        _ = isPrimaryOffice;

        Assert.True(BicParser.IsValid(input.AsSpan()));
    }

    [Theory]
    [MemberData(nameof(BicParserValidBicTests.WellFormedBics), MemberType = typeof(BicParserValidBicTests))]
    public void Parse_span_extracts_the_same_segments_as_the_string_overload(
        string input, string institution, string country, string location, string branch, bool isTestBic, bool isPrimaryOffice)
    {
        _ = isTestBic;
        _ = isPrimaryOffice;

        var fromSpan = BicParser.Parse(input.AsSpan());
        var fromString = BicParser.Parse(input);

        Assert.Equal(institution, fromSpan.Institution);
        Assert.Equal(country, fromSpan.Country);
        Assert.Equal(location, fromSpan.Location);
        Assert.Equal(branch, fromSpan.Branch);
        Assert.Equal(fromString, fromSpan);
    }

    [Theory]
    [MemberData(nameof(BicParserValidBicTests.WellFormedBics), MemberType = typeof(BicParserValidBicTests))]
    public void TryParse_span_returns_true_and_matches_the_string_overload(
        string input, string institution, string country, string location, string branch, bool isTestBic, bool isPrimaryOffice)
    {
        _ = institution;
        _ = country;
        _ = location;
        _ = branch;
        _ = isTestBic;
        _ = isPrimaryOffice;

        var parsed = BicParser.Parse(input);

        Assert.True(BicParser.TryParse(input.AsSpan(), out var tried));
        Assert.Equal(parsed, tried);
    }

    [Theory]
    [MemberData(nameof(BicParserInvalidBicTests.MalformedBics), MemberType = typeof(BicParserInvalidBicTests))]
    public void IsValid_span_returns_false_for_malformed_bics(string? input)
    {
        Assert.False(BicParser.IsValid(input.AsSpan()));
    }

    [Theory]
    [MemberData(nameof(BicParserInvalidBicTests.MalformedBics), MemberType = typeof(BicParserInvalidBicTests))]
    public void TryParse_span_returns_false_and_null_for_malformed_bics(string? input)
    {
        var succeeded = BicParser.TryParse(input.AsSpan(), out var result);

        Assert.False(succeeded);
        Assert.Null(result);
    }

    [Theory]
    [MemberData(nameof(BicParserInvalidBicTests.MalformedBics), MemberType = typeof(BicParserInvalidBicTests))]
    public void Parse_span_throws_the_same_exception_type_as_the_string_overload(string? input)
    {
        Assert.Throws<BicFormatException>(() => BicParser.Parse(input.AsSpan()));
    }

    [Fact]
    public void IsValid_span_rejects_the_kosovo_xk_country_code()
    {
        // Kosovo's XK is a genuine, SWIFT-registered BIC country code but is user-assigned, not an
        // official ISO 3166-1 alpha-2 code, so strict validation rejects it. This must hold for the
        // span overload exactly as for the string overload.
        Assert.False(BicParser.IsValid("RBKOXKPR".AsSpan()));
        Assert.False(BicParser.IsValid("RBKOXKPR"));
    }

    [Fact]
    public void IsValid_span_accepts_test_bic_marked_by_zero_in_the_location_code()
    {
        Assert.True(BicParser.IsValid("TESTGBA0".AsSpan()));
        Assert.True(BicParser.Parse("TESTGBA0".AsSpan()).IsTestBic);
    }

    [Fact]
    public void IsValid_span_sliced_from_a_larger_buffer_validates_without_a_substring()
    {
        // A BIC embedded in a larger message buffer, the shape an MT940/MT103 field arrives in.
        const string message = "...GENODEF1S02...";
        const int bicOffset = 3;
        const int bicLength = BicFormat.BranchLength;

        Assert.True(BicParser.IsValid(message.AsSpan(bicOffset, bicLength)));

        var parsed = BicParser.Parse(message.AsSpan(bicOffset, bicLength));
        Assert.Equal("GENO", parsed.Institution);
        Assert.Equal("DE", parsed.Country);
        Assert.Equal("F1", parsed.Location);
        Assert.Equal("S02", parsed.Branch);
    }

    [Fact]
    public void IsValid_span_from_a_buffer_slice_allocates_less_than_the_substring_path()
    {
        const string message = "...GENODEF1S02...";
        const int bicOffset = 3;
        const int bicLength = BicFormat.BranchLength;

        // Warm up both paths so first-call JIT allocations do not skew the measurement.
        _ = BicParser.IsValid(message.AsSpan(bicOffset, bicLength));
        _ = BicParser.IsValid(message.Substring(bicOffset, bicLength));

        var beforeSpan = GC.GetAllocatedBytesForCurrentThread();
        _ = BicParser.IsValid(message.AsSpan(bicOffset, bicLength));
        var spanBytes = GC.GetAllocatedBytesForCurrentThread() - beforeSpan;

        var beforeSubstring = GC.GetAllocatedBytesForCurrentThread();
        _ = BicParser.IsValid(message.Substring(bicOffset, bicLength));
        var substringBytes = GC.GetAllocatedBytesForCurrentThread() - beforeSubstring;

        // The substring path does everything the span path does plus allocate the sliced string,
        // so slicing straight into the span overload is strictly cheaper.
        Assert.True(spanBytes < substringBytes, $"span={spanBytes} substring={substringBytes}");
    }

    [Fact]
    public void Parse_span_on_invalid_input_throws_the_same_exception_type_as_parse_string()
    {
        var fromString = Assert.Throws<BicFormatException>(() => BicParser.Parse("DEUTDEF"));
        var fromSpan = Assert.Throws<BicFormatException>(() => BicParser.Parse("DEUTDEF".AsSpan()));

        Assert.Equal(fromString.GetType(), fromSpan.GetType());
        Assert.Equal(fromString.Message, fromSpan.Message);
    }

    [Fact]
    public void Null_string_is_still_handled_exactly_as_before()
    {
        Assert.False(BicParser.IsValid((string?)null));
        Assert.False(BicParser.TryParse((string?)null, out var result));
        Assert.Null(result);
        Assert.Throws<BicFormatException>(() => BicParser.Parse(null!));
    }
}
