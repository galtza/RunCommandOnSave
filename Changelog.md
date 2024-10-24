# Changelog

## [v1.1.1] - 2024-10-24

### Fixed
- Fixed an issue that was making the binary downloaded from the marketplace to be the old binary

## [v1.1.0] - 2024-10-18

### Added
- New ability to specify commands per extension
- `.runcommandonsave` files caching (based on size/last modified time instead of hash)
- Custom INI file reader

### Changed
- Normalized CR via `.gitattributes`
- Updated `.gitignore` to include Visual Studio log file
- Output pane name now contains the version number

### Fixed
- Manifest for Visual Studio 2022 and 2019

