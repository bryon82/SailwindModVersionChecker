# Changelog

All notable changes to this project will be documented in this file.

## [v1.1.3] - 2025-04-02

### Added
- Config entry to disable checking for updates

## [v1.1.2] - 2025-04-02

### Fixed
- Release directory structure

## [v1.1.1] - 2025-04-02

### Added
- Sanitization for Thunderstore website url to visit

### Changed
- Use UnityEngine.Application.OpenURL instead of System.Diagnostics.Process.Start to open mod repo website

## [v1.1.0] - 2025-04-02

### Changed
- To get a mod's repo website, MVC now uses a list hosted on its own GitHub repo

## [v1.0.2] - 2025-03-31

### Added
- Logging for up to date mods

## [v1.0.1] - 2025-03-31

### Added
- Checking of mod versions against their release versions on either GitHub or Thunderstore websites.
- A notification on game startup if there are any updates to installed mods.
- Button to visit mod website to download updated mod.
- Configuration to enable/disable notification.
