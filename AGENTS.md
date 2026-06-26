# AGENTS.md

## Project overview

This repository is a Unity UPM package for converting GPS coordinates to Unity coordinates.

The package must support two transformation modes:

1. Local ENU mode
   - Convert WGS84 latitude, longitude, and ellipsoidal height to ECEF.
   - Convert ECEF delta from a fixed origin to local ENU.
   - Map ENU to Unity coordinates as:
     - Unity X = East
     - Unity Y = Up
     - Unity Z = North

2. Japan Plane Rectangular Coordinate System mode
   - Convert latitude and longitude to Japan Plane Rectangular Coordinates using the official GSI Gauss-Krüger / transverse Mercator formulas.
   - Support Japanese coordinate system zones I through XIX.
   - Map projected coordinates to Unity coordinates as:
     - Unity X = projected Y, east
     - Unity Y = height or height delta
     - Unity Z = projected X, north

## Product requirements

- The public API must be simple:
  - Set or pass an origin GPS coordinate.
  - Convert any GPS coordinate to `UnityEngine.Vector3`.
  - Allow choosing the transformation mode.
  - Allow choosing Japan Plane Rectangular zone when that mode is used.
- Runtime code must be usable in Unity player builds.
- Do not depend on editor-only APIs from runtime code.
- Do not add runtime third-party dependencies unless explicitly requested.
- All geodetic calculations must use `double` internally.
- Convert to `UnityEngine.Vector3` only at the final API boundary.
- Keep the API stable, small, and documented.

## Accuracy requirements

- Local ENU mode must use WGS84 ellipsoid constants by default:
  - semi-major axis a = 6378137.0
  - inverse flattening = 298.257223563
- Japan Plane Rectangular mode should use GRS80/JGD-compatible constants where appropriate:
  - semi-major axis a = 6378137.0
  - inverse flattening = 298.257222101
- Do not implement simple spherical approximations for the production conversion path.
- Add tests for:
  - origin converts to zero.
  - small east/north/up offsets have expected sign and magnitude.
  - Unity axis mapping is correct.
  - Japan Plane Rectangular zone conversion agrees with reference values within a documented tolerance.

## Unity package requirements

- Follow Unity Package Manager layout:
  - `package.json`
  - `Runtime/`
  - `Tests/Runtime/`
  - `Samples~/`
  - `Documentation~/`
  - `README.md`
  - `CHANGELOG.md`
  - `LICENSE.md`
- Runtime code must be under `Runtime/`.
- Runtime tests must be under `Tests/Runtime/`.
- Add `.asmdef` files for runtime and tests.
- Use NUnit-compatible Unity Test Framework tests.
- Avoid unsafe code.

## Coding style

- Use namespace `ntk.GeospatialCoordinates`.
- Prefer immutable structs for coordinate values.
- Validate constructor and factory inputs.
- Throw `ArgumentOutOfRangeException` for invalid latitude, longitude, height, or zone.
- Throw `InvalidOperationException` only for invalid object state.
- Use XML documentation comments on public APIs.
- Keep implementation classes small and testable.

## Testing expectations

After modifying runtime code, run or prepare instructions for:

- Unity Test Framework runtime tests.
- C# compile check inside Unity.
- Package import check from a local Unity project.

If Unity cannot be run in the current environment, still create tests and document the exact command or Unity Editor flow needed to run them.

## Documentation expectations

Update documentation when public API changes:

- `README.md`
- `Documentation~/index.md`
- `Samples~/BasicUsage/README.md`
- `CHANGELOG.md`

Documentation must include:
- Local ENU usage.
- Japan Plane Rectangular usage.
- Unity axis mapping.
- Height handling.
- Precision notes for RTK-GPS and 1–5 km scale use.