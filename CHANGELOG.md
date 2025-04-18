# Changelog

All notable changes to this project will be documented in this file.

## [v1.2.2] - 2025-04-14

### Updated
- Code refactor for better handling of nulls

### Fixed
- Async error on startup caused by race condition if updates ui element is not instantiated in time 

## [v1.2.1] - 2025-04-14

### Fixed
- Async error that would get sometimes thrown during startup
- Mod updates notification persisting when clicking on one of the start menu buttons

## [v1.2.0] - 2025-04-13

### Changed
- The way individual mods versions are checked has been overhauled. Now the release versions are checked by a github action and the list of versions are populated in a json file on this repo. This makes startup a little faster as every time the game starts up it is no longer pinging every installed mods release pages.

## [v1.1.3] - 2025-04-13

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
