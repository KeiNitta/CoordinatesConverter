# Geospatial Coordinates

Unity UPM package for GPS-to-Unity conversion. Runtime code has no third-party dependencies.

## Install

In Unity Package Manager, add this repository from disk or Git. The package name is `com.ntk.geospatial-coordinates`.

## Local ENU

```csharp
var origin = new GpsCoordinate(35.681236d, 139.767125d, 40d);
var converter = new CoordinateConverter(origin);
Vector3 position = converter.ToUnity(new GpsCoordinate(35.681300d, 139.767200d, 42d));
```

This uses WGS84 ECEF followed by a local ENU transform. Unity axes are `X = east`, `Y = up`, `Z = north`.

## Japan Plane Rectangular

```csharp
var converter = new CoordinateConverter(origin,
    CoordinateTransformationMode.JapanPlaneRectangular,
    JapanPlaneRectangularZone.IX);
Vector3 position = converter.ToUnity(gps);
```

The GRS80/JGD-compatible GSI transverse-Mercator formulation supports zones I through XIX. Unity maps `X = projected easting`, `Y = gps.HeightMeters - origin.HeightMeters`, and `Z = projected northing`. Plane coordinates are referenced to the selected zone origin, not the supplied converter origin.

## Precision

All geodetic work uses `double`; `Vector3` is created only at the public boundary. For RTK-GPS use, keep Unity scenes near their working origin. Standard `float` positions lose useful sub-centimetre detail as distance from the Unity origin grows; rebasing is recommended across 1–5 km or when stringent local precision is required.

For an integration test run without changing the consuming project's manifest, import the **Runtime Tests** sample from Package Manager. The sample appears in Unity Test Runner's EditMode tab. Sample documentation is kept in `Documentation~`, so importing a sample does not add Markdown assets or their `.meta` files to the consuming project. See [Documentation~](Documentation~/index.md).
