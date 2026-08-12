# Bic.NET

Validate and parse SWIFT BIC (ISO 9362) business identifier codes in .NET. Zero external dependencies.

A BIC (also called a SWIFT code) identifies a bank or financial institution in international
payments: `DEUTDEFF` addresses Deutsche Bank AG's head office in Frankfurt, `DEUTDEFF500`
addresses one of its branches. Every IBAN payment, SWIFT MT message, and correspondent-banking
integration carries one, and getting the structure wrong (an 11-character branch code truncated to
8, a country code that is not actually assigned, a lower-case code failing a strict comparison)
produces a payment sent to the wrong place or rejected outright. On NuGet the BIC validators that
exist are either bundled inside large, opinionated banking suites or unmaintained. Bic.NET is a
small, dependency-free library that does one thing: turn a string into a correctly parsed,
correctly validated BIC, or reject it with a clear reason.

## Install

```
dotnet add package Bic.Net
```

## Quickstart

```csharp
using Bic;

BicParser.IsValid("DEUTDEFF");     // true
BicParser.IsValid("DEUTDEFF500");  // true
BicParser.IsValid("DEUTUKFF");     // false: "UK" is not an ISO 3166-1 country code (it's "GB")

BicCode bic = BicParser.Parse("DEUTDEFF500");
// bic.Institution     == "DEUT"
// bic.Country         == "DE"
// bic.Location        == "FF"
// bic.Branch          == "500"
// bic.IsPrimaryOffice == false
// bic.IsTestBic       == false
```

## Comparing a head office BIC against its branches

An 8-character BIC and its 11-character `...XXX` equivalent both mean "primary office", so a naive
string comparison treats them as different values. `BicCode.CanonicalValue` normalizes both to the
same 11-character form:

```csharp
using Bic;

var headOffice = BicParser.Parse("DEUTDEFF");
var explicitPrimaryOffice = BicParser.Parse("DEUTDEFFXXX");

headOffice.CanonicalValue == explicitPrimaryOffice.CanonicalValue; // true: both "DEUTDEFFXXX"
headOffice.Value == explicitPrimaryOffice.Value;                   // false: "DEUTDEFF" vs "DEUTDEFFXXX"
headOffice.IsPrimaryOffice;                                        // true
```

## Non-throwing validation for user input

```csharp
using Bic;

string? input = GetBicFromForm();

if (BicParser.TryParse(input, out BicCode? bic))
{
    SaveCounterpartyBic(bic!.CanonicalValue);
}
else
{
    ShowValidationError("Enter a valid SWIFT/BIC code.");
}
```

`Parse` throws `BicFormatException` (a `FormatException`) with a message describing exactly which
segment failed and why, useful for logs and for surfacing validation errors during development.

## API

| Member | Purpose |
|---|---|
| `BicParser.IsValid(string?)` | Structural and country-code validity check; never throws |
| `BicParser.Parse(string)` | Parses into a `BicCode`; throws `BicFormatException` on invalid input |
| `BicParser.TryParse(string?, out BicCode?)` | Non-throwing parse |
| `BicCode.Institution` / `.Country` / `.Location` / `.Branch` | The four ISO 9362 segments, upper case |
| `BicCode.IsTestBic` | `true` when the second location-code character is `0` |
| `BicCode.IsPrimaryOffice` | `true` when the branch segment is empty or `XXX` |
| `BicCode.Value` | Normalized BIC at its original length (8 or 11 characters) |
| `BicCode.CanonicalValue` | Normalized BIC always at 11 characters, head office filled in as `XXX` |
| `IsoCountryCodes.Contains(string)` / `.All` | The ISO 3166-1 alpha-2 codes this library validates against |
| `BicFormat` | The length and offset constants that define the ISO 9362 layout |

## What gets validated

A BIC is 8 or 11 characters: a 4-letter institution code, a 2-letter ISO 3166-1 alpha-2 country
code, a 2-character alphanumeric location code, and an optional 3-character alphanumeric branch
code. Bic.NET checks, in order:

- Overall length is exactly 8 or 11 characters.
- The institution code is 4 ASCII letters.
- The country code is 2 ASCII letters *and* a country code actually assigned by ISO 3166-1, not
  just any two letters. `DEUTUKFF` fails even though "UK" looks plausible, because the assigned
  code for the United Kingdom is `GB`. Withdrawn codes such as `AN` (Netherlands Antilles) and
  user-assigned codes such as `ZZ` are rejected the same way.
- The location code is 2 ASCII letters or digits.
- The branch code, when present, is 3 ASCII letters or digits.

Input is normalized case-insensitively: `deutdeff` and `DEUTDEFF` parse identically.

## Why this exists

There is no check digit or checksum in a BIC (unlike an IBAN), so "verifying" a BIC really means
verifying its structure and its country code against the real ISO 3166-1 list, correctly, every
time. That is a small amount of logic that is easy to get subtly wrong (off-by-one segment offsets,
accepting `UK`, rejecting lower case, mishandling the 8-vs-11-character equivalence) and Bic.NET
gets it right once so every consumer does not have to re-derive it. It pairs with
[Mt940.Net](https://github.com/IsraelIyonsi/Mt940.NET) and the rest of the reconciliation and AML
tooling arc: parsed BICs are a routine field inside SWIFT MT940/MT103 messages and payment
reconciliation records.

## Dependencies and AOT

Zero runtime dependencies. The library targets `net8.0`, uses only in-box BCL types
(`System.Collections.Frozen.FrozenSet<T>` for the country-code set, `System.Span<T>` for allocation-light
parsing), and does no reflection, dynamic code generation, or I/O, so it is fully compatible with
Native AOT and trimming.

## Notes and limitations

- The ISO 3166-1 alpha-2 country-code set is embedded at the version documented in the changelog.
  ISO occasionally assigns or withdraws codes; this library is not a live feed from the ISO 3166
  Maintenance Agency.
- Bic.NET validates BIC *structure*, not institution registration. A structurally valid BIC such as
  `AAAAGB2L` passes validation even if `AAAA` is not an institution actually registered with SWIFT;
  confirming registration requires a live SWIFT BIC directory lookup, which is out of scope for a
  dependency-free offline library.
- The example BICs used in this library's own test suite beyond the canonical `DEUTDEFF` /
  `DEUTDEFF500` pairing (such as `BARCGB22`, `CHASUS33`, `NEDSZAJJ`) are widely published examples
  reused across BIC-validation libraries; they are not re-verified against a live SWIFT directory.
- **Kosovo (`XK`) is rejected.** SWIFT registers live, production BICs with country code `XK` (for
  example `RBKOXKPR`, Raiffeisen Bank Kosovo), but `XK` is a user-assigned code, not one officially
  allocated by ISO 3166-1. Because Bic.NET validates strictly against the ISO 3166-1 alpha-2 set,
  `IsValid`/`Parse` reject these genuine, registered BICs. This is correct behavior for the ISO
  3166-1 standard as specified, but it means an MT940/reconciliation pipeline that may see real
  Kosovo counterparties needs its own allowance for `XK` on top of this library rather than relying
  on Bic.NET to accept it.

## License

MIT. See [LICENSE](LICENSE).
