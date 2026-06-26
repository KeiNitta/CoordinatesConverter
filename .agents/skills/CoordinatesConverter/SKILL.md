
---

## `SKILL.md`

`AGENTS.md` は常時ルール、`SKILL.md` はこの種の実装作業をさせるときの作業手順です。Codexはリポジトリ内の `.agents/skills` にあるSkillを使えます。:contentReference[oaicite:2]{index=2}

```md
---
name: unity-coordinates-converter
description: Use this skill when implementing, reviewing, or extending the CoordinatesConverter Unity UPM package for GPS to Unity coordinate conversion.
---

# Unity Coordinates Converter Skill

## Goal

Implement and maintain a Unity UPM package named `CoordinatesConverter`.

The package converts GPS latitude, longitude, and height into Unity `Vector3` coordinates.

Supported modes:

1. Local ENU
2. Japan Plane Rectangular Coordinate System

## Required workflow

When using this skill:

1. Inspect the current repository structure.
2. Preserve Unity UPM package layout.
3. Implement runtime code under `Runtime/`.
4. Implement tests under `Tests/Runtime/`.
5. Use `double` internally for all coordinate calculations.
6. Return `UnityEngine.Vector3` only at the final Unity-facing API boundary.
7. Avoid runtime third-party dependencies.
8. Update README, sample, documentation, and changelog when public behavior changes.

## Local ENU mode

Implement:

- GPS latitude, longitude, ellipsoidal height to ECEF.
- ECEF delta from origin.
- ECEF delta to ENU.
- Unity mapping:
  - Unity X = East
  - Unity Y = Up
  - Unity Z = North

Use WGS84 constants by default:

- semi-major axis: 6378137.0
- inverse flattening: 298.257223563

## Japan Plane Rectangular mode

Implement:

- Zones I through XIX.
- GSI-compatible forward projection.
- Projected X as northing.
- Projected Y as easting.
- Unity mapping:
  - Unity X = projected Y delta
  - Unity Y = height delta
  - Unity Z = projected X delta

Use:

- semi-major axis: 6378137.0
- inverse flattening: 298.257222101
- central scale factor: 0.9999

## Public API target

Prefer this shape unless a better API already exists:

```csharp
namespace CoordinatesConverter
{
    public enum CoordinateTransformMode
    {
        LocalEnu,
        JapanPlaneRectangular
    }

    public readonly struct GpsCoordinate
    {
        public double LatitudeDeg { get; }
        public double LongitudeDeg { get; }
        public double HeightMeters { get; }
    }

    public sealed class CoordinatesConverter
    {
        public static CoordinatesConverter Create(
            GpsCoordinate origin,
            CoordinateTransformMode mode,
            JapanPlaneRectangularZone? zone = null);

        public Vector3 ToUnityPosition(GpsCoordinate coordinate);
    }
}