using Bic;

namespace Bic.Net.Tests;

public class BicParserInvalidBicTests
{
    public static TheoryData<string?> MalformedBics() => new()
    {
        // Null, empty, and whitespace.
        null,
        "",
        " ",
        "        ",

        // Wrong overall length: neither 8 nor 11 characters.
        "DEUTDEF",           // 7
        "DEUTDEFF ",         // 9, trailing space
        "DEUTDEFFX",         // 9
        "DEUTDEFFXX",        // 10
        "DEUTDEFFXXXX",      // 12
        "D",                 // 1
        "DEUTDEFF50000",     // 13

        // Non-letter character in the 4-letter institution code.
        "1EUTDEFF",
        "DE3TDEFF",
        "DEU1DEFF",
        "DEUT DEFF",

        // Non-letter character in the 2-letter country code.
        "DEUT1EFF",
        "DEUTD3FF",
        "DEUT-EFF",

        // Country code that is well-formed (two letters) but not a recognized ISO 3166-1
        // alpha-2 code.
        "DEUTUKFF",  // "UK" is a common mistake; the ISO 3166-1 code for the United Kingdom is "GB".
        "DEUTZZFF",  // "ZZ" is in the user-assigned range, never officially allocated.
        "DEUTQQFF",  // "QQ" is in the user-assigned range, never officially allocated.
        "DEUTANFF",  // "AN" (Netherlands Antilles) was withdrawn from ISO 3166-1 in 2010.

        // Non-alphanumeric character in the 2-character location code.
        "DEUTDE*F",
        "DEUTDE F",
        "DEUTDE.F",

        // Non-alphanumeric character in the optional 3-character branch code.
        "DEUTDEFF5*0",
        "DEUTDEFF5 0",
        "DEUTDEFF5#0",
    };

    [Theory]
    [MemberData(nameof(MalformedBics))]
    public void IsValid_returns_false_for_malformed_bics(string? input)
    {
        Assert.False(BicParser.IsValid(input));
    }

    [Theory]
    [MemberData(nameof(MalformedBics))]
    public void TryParse_returns_false_and_null_for_malformed_bics(string? input)
    {
        var succeeded = BicParser.TryParse(input, out var result);

        Assert.False(succeeded);
        Assert.Null(result);
    }

    [Theory]
    [MemberData(nameof(MalformedBics))]
    public void Parse_throws_BicFormatException_for_malformed_bics(string? input)
    {
        Assert.Throws<BicFormatException>(() => BicParser.Parse(input!));
    }

    [Fact]
    public void Parse_error_message_reports_the_offending_length()
    {
        var exception = Assert.Throws<BicFormatException>(() => BicParser.Parse("DEUTDEF"));

        Assert.Contains("7", exception.Message);
        Assert.Contains("8", exception.Message);
        Assert.Contains("11", exception.Message);
    }

    [Fact]
    public void Parse_error_message_reports_an_unrecognized_country_code()
    {
        var exception = Assert.Throws<BicFormatException>(() => BicParser.Parse("DEUTUKFF"));

        Assert.Contains("ISO 3166-1", exception.Message);
    }
}
