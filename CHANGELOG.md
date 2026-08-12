# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2026-08-12

### Added

- `BicParser` static API: `IsValid(string?)`, `Parse(string)`, and non-throwing `TryParse(string?, out BicCode?)`.
- `BicCode` record exposing `Institution`, `Country`, `Location`, `Branch`, `IsTestBic`, `IsPrimaryOffice`, `Value` (normalized at source length), and `CanonicalValue` (always 11 characters, head office filled in as `XXX`).
- Full ISO 9362 structural validation: exact 8-or-11-character length, letters-only institution and country segments, alphanumeric location and branch segments, and a country segment validated against a shipped, embedded ISO 3166-1 alpha-2 code set (`IsoCountryCodes`) rather than a bare letter check.
- Test-BIC detection via the second character of the location code (`BicFormat.TestBicIndicator`).
- `BicFormat` public constants documenting every length and offset in the ISO 9362 layout.
- Descriptive `BicFormatException` (derived from `FormatException`) identifying which segment failed and why.
- Verified against the canonical ISO 9362 worked example (`DEUTDEFF` / `DEUTDEFF500`) plus widely published example BICs, alongside a comprehensive set of malformed-input negatives (wrong length, non-letter institution/country characters, non-alphanumeric location/branch characters, unassigned or withdrawn country codes such as `UK` and `AN`).
- Zero runtime dependencies; built on the in-box `System.Collections.Frozen` and `System.Span<T>`.
