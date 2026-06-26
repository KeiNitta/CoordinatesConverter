# Geospatial Coordinates

`CoordinateConverter` exposes `ToUnity(GpsCoordinate)`. Local ENU holds a fixed origin; Japan Plane Rectangular uses only its selected zone and datum.

Local ENU uses WGS84 (`a = 6378137.0`, inverse flattening `298.257223563`) and maps east/up/north to Unity X/Y/Z.

Japan Plane Rectangular mode uses the GSI Gauss-Krüger transverse-Mercator series with GRS80 inverse flattening `298.257222101`, scale factor 0.9999, and zones I–XIX. Create it with `CoordinateConverter.CreateJapanPlaneRectangular(zone, datum)`; it has no local origin. It maps projected east/north to Unity X/Z. Y is the input coordinate's ellipsoidal height.

Use `JapanPlaneRectangularDatum.Jgd2011Grs80` for normal WGS84/GNSS input. For source coordinates explicitly labelled as the legacy Japanese geodetic datum (Tokyo Datum), select `JapanPlaneRectangularDatum.TokyoDatumBessel`, which uses the Bessel ellipsoid. In Tokyo Datum mode, the input latitude and longitude must already be Tokyo Datum values; converting WGS84 values first requires a separate datum transformation.

Calculation reference: [Geospatial Information Authority of Japan (GSI), plane rectangular coordinate system reference PDF (Japanese)](https://www.gsi.go.jp/common/000061216.pdf). This package is an independent implementation, not GSI software.

The supplied zone-IX reference test uses the GSI forward-projection example and permits 1 m to account for published-value rounding. The test suite also verifies the published origins of zones I–XIX with both supported projection datums. For a consuming Unity project, import the **Runtime Tests** sample in Package Manager; Unity places it beneath `Assets/Samples`, where it appears automatically in **Window > General > Test Runner > EditMode**. This does not require a consuming-project manifest modification. Confirm compile status in the Console and, for import validation, add the package by local disk path in Package Manager.

All computation before `Vector3` is `double`. Unity `Vector3` is float, so use a nearby origin and rebase scenes for high-precision RTK workflows spanning 1–5 km.

## Samples

The **Basic Usage** sample contains only `BasicUsageExample.cs`; assign its `target` field to apply a converted ENU position at startup.

Import the **Runtime Tests** sample from Package Manager, then run its tests through **Window > General > Test Runner > EditMode**.

To run the package's `Tests/Runtime` assembly in package-development CI, add `com.ntk.geospatial-coordinates` to the consuming project's `testables` array in `Packages/manifest.json`. `testables` contains UPM package names, not assembly names. Fail CI if the reported test count is zero.

## GameObject component

Add **Geospatial Coordinates > Coordinate Converter** to any GameObject. `CoordinateConverterComponent` exposes origin latitude, longitude, ellipsoidal height, mode, and Japan zone in the Inspector. Its **Current Geographic Coordinate** fields accept the point to convert. Local ENU interprets latitude and longitude as WGS84. Japan Plane Rectangular interprets them as the selected datum: use normal WGS84/GNSS input for JGD2011 / GRS80, and coordinates already expressed in Tokyo Datum for Tokyo Datum / Bessel. Assign **Target** and enable **Apply Current Coordinate On Start** to set its `localPosition` automatically in Play Mode. If Target is empty, the component moves its own GameObject. From code, call `ToUnity(GpsCoordinate)`, `SetCurrentCoordinate`, `ConvertCurrentCoordinate`, or `ApplyCurrentCoordinate`. If changing the origin, mode, or zone at runtime, call `Rebuild()` before converting the next point.

The Inspector enables **Japan Zone** and **Datum** only when the mode is **Japan Plane Rectangular**; in Local ENU they remain visible but disabled. Japan Plane Rectangular hides Origin because that mode does not use it. Local ENU shows Origin.
