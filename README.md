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

## GameObject component

Add **Geospatial Coordinates > Coordinate Converter** to a GameObject. Set the origin, mode, and Japan zone in the Inspector, then convert from another component:

```csharp
[SerializeField] private CoordinateConverterComponent converterComponent;

var position = converterComponent.ToUnity(35.681300d, 139.767200d, 42d);
```

The component also has **Current Geographic Coordinate** fields. In **Local ENU** mode, enter WGS84 latitude and longitude. In **Japan Plane Rectangular** mode, latitude and longitude must use the selected **Datum**: normal WGS84/GNSS values for **JGD2011 / GRS80**, or coordinates already expressed in **Tokyo Datum** for **Tokyo Datum / Bessel**. The component does not transform between datums. Assign an optional **Target**, and enable **Apply Current Coordinate On Start** to set its `localPosition` automatically when Play starts. If Target is empty, the component's own GameObject is moved. From code, use `SetCurrentCoordinate(gps)`, `ConvertCurrentCoordinate()`, or `ApplyCurrentCoordinate()`.

**Japan Zone** and **Datum** are enabled only when the mode is **Japan Plane Rectangular**. They remain visible but disabled in Local ENU, making it clear that they are not used. Japan Plane Rectangular hides Origin because its X/Y/Z does not use it. Local ENU uses Origin.

Call `Rebuild()` after changing the origin, mode, or zone from code. `ToUnity` rebuilds automatically if it is called before `Awake`.

## Japan Plane Rectangular

```csharp
var converter = CoordinateConverter.CreateJapanPlaneRectangular(JapanPlaneRectangularZone.IX);
Vector3 position = converter.ToUnity(gps);
```

The GRS80/JGD-compatible GSI transverse-Mercator formulation supports zones I through XIX. Unity maps `X = projected easting`, `Y = gps.HeightMeters`, and `Z = projected northing`. Plane coordinates are referenced to the selected zone origin and do not use a converter origin. The legacy constructor that accepts an origin remains compatible, but ignores that value in this mode.

Select `JapanPlaneRectangularDatum.Jgd2011Grs80` (the default) for ordinary WGS84/GNSS input. Select `JapanPlaneRectangularDatum.TokyoDatumBessel` only when the input latitude and longitude are explicitly in the legacy Japanese geodetic datum (Tokyo Datum). In Tokyo Datum mode, do not pass WGS84 coordinates unless you transform them first. Datum selection changes the projection ellipsoid; it does not transform coordinates between datums.

## References

Japan Plane Rectangular implementation reference: [河瀬和重 (2011): Gauss-Krüger投影における経緯度座標及び平面直角座標相互間の座標換算についての
より簡明な計算方法，国土地理院時報，121，109–124，doi: 10.57499/JOURNAL_121_12](https://www.gsi.go.jp/common/000061216.pdf). The implementation uses the document's GRS80-compatible ellipsoid, the `0.9999` scale factor, zone origins I–XIX, and the Gauss-Krüger / transverse-Mercator forward-projection formulation. This package is an independent implementation and is not affiliated with GSI.

## Precision

All geodetic work uses `double`; `Vector3` is created only at the public boundary. For RTK-GPS use, keep Unity scenes near their working origin. Standard `float` positions lose useful sub-centimetre detail as distance from the Unity origin grows; rebasing is recommended across 1–5 km or when stringent local precision is required.

For an integration test run, import the **Runtime Tests** sample from Package Manager and run it from Unity Test Runner's EditMode tab. This does not require a consuming-project manifest change.

For package development or CI that runs `Tests/Runtime` directly, add the package name—not its test assembly name—to the consuming project's `Packages/manifest.json`:

```json
"testables": [
  "com.ntk.geospatial-coordinates"
]
```

Require a non-zero test count in CI. See [Documentation~](Documentation~/index.md).
