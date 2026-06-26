# Geospatial Coordinates

`CoordinateConverter` holds a fixed `GpsCoordinate` origin and exposes `ToUnity(GpsCoordinate)`.

Local ENU uses WGS84 (`a = 6378137.0`, inverse flattening `298.257223563`) and maps east/up/north to Unity X/Y/Z.

Japan Plane Rectangular mode uses the GSI Gauss-Krüger transverse-Mercator series with GRS80 inverse flattening `298.257222101`, scale factor 0.9999, and zones I–XIX. It maps projected east/north to Unity X/Z. Y is the coordinate ellipsoidal-height difference from the converter origin.

The supplied zone-IX reference test uses the GSI forward-projection example and permits 1 m to account for published-value rounding. For a consuming Unity project, import the **Runtime Tests** sample in Package Manager; Unity places it beneath `Assets/Samples`, where it appears automatically in **Window > General > Test Runner > EditMode**. This does not require a consuming-project manifest modification. Confirm compile status in the Console and, for import validation, add the package by local disk path in Package Manager.

All computation before `Vector3` is `double`. Unity `Vector3` is float, so use a nearby origin and rebase scenes for high-precision RTK workflows spanning 1–5 km.

## Samples

The **Basic Usage** sample contains only `BasicUsageExample.cs`; assign its `target` field to apply a converted ENU position at startup.

The **Runtime Tests** sample contains only the test source and asmdef required by Unity Test Runner. Import it from Package Manager, then run the four tests through **Window > General > Test Runner > EditMode**. No Markdown files are included in either sample, so Unity does not generate `.meta` files for sample documentation on import.
