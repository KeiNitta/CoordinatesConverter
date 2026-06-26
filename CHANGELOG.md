# Changelog

## Unreleased

- Added an origin-free Japan Plane Rectangular factory; this mode no longer computes a local WGS84 origin internally.
- Clarified that Japan Plane Rectangular input latitude and longitude must use the selected datum, including Tokyo Datum input.
- Restricted npm package contents to Unity package assets.
- Removed the ineffective assembly-name `testables` entry and documented the consuming-project setup required for package-development CI.
- Added zone-origin tests for all I–XIX Japan Plane Rectangular zones and both supported projection datums.

## 1.0.0 - 2026-06-26

- Initial UPM package with WGS84 local ENU conversion.
- Added GRS80/JGD-compatible Japan Plane Rectangular zones I–XIX.
- Added runtime tests, samples, and documentation.
