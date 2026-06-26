using System;

namespace ntk.GeospatialCoordinates
{
    /// <summary>
    /// Geographic coordinate expressed in degrees and ellipsoidal metres.
    /// Local ENU interprets it as WGS84; Japan Plane Rectangular interprets latitude and longitude using the selected <see cref="JapanPlaneRectangularDatum"/>.
    /// </summary>
    public readonly struct GpsCoordinate
    {
        /// <summary>Latitude in degrees, in the inclusive range [-90, 90].</summary>
        public double LatitudeDegrees { get; }
        /// <summary>Longitude in degrees, in the inclusive range [-180, 180].</summary>
        public double LongitudeDegrees { get; }
        /// <summary>Ellipsoidal height in metres.</summary>
        public double HeightMeters { get; }

        /// <summary>Creates a validated geographic coordinate.</summary>
        public GpsCoordinate(double latitudeDegrees, double longitudeDegrees, double heightMeters = 0d)
        {
            if (!IsFinite(latitudeDegrees) || latitudeDegrees < -90d || latitudeDegrees > 90d)
                throw new ArgumentOutOfRangeException(nameof(latitudeDegrees));
            if (!IsFinite(longitudeDegrees) || longitudeDegrees < -180d || longitudeDegrees > 180d)
                throw new ArgumentOutOfRangeException(nameof(longitudeDegrees));
            if (!IsFinite(heightMeters))
                throw new ArgumentOutOfRangeException(nameof(heightMeters));

            LatitudeDegrees = latitudeDegrees;
            LongitudeDegrees = longitudeDegrees;
            HeightMeters = heightMeters;
        }

        private static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
    }
}
