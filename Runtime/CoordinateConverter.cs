using System;
using UnityEngine;

namespace ntk.GeospatialCoordinates
{
    /// <summary>Converts validated GPS coordinates to Unity world coordinates relative to a fixed origin.</summary>
    public sealed class CoordinateConverter
    {
        private const double DegreesToRadians = Math.PI / 180d;
        private const double Wgs84SemiMajorAxis = 6378137d;
        private const double Wgs84InverseFlattening = 298.257223563d;
        private const double Grs80InverseFlattening = 298.257222101d;
        private const double BesselSemiMajorAxis = 6377397.155d;
        private const double BesselInverseFlattening = 299.152813d;
        private readonly GpsCoordinate origin;
        private readonly double originEcefX;
        private readonly double originEcefY;
        private readonly double originEcefZ;
        private readonly double sinOriginLatitude;
        private readonly double cosOriginLatitude;
        private readonly double sinOriginLongitude;
        private readonly double cosOriginLongitude;
        private readonly JapanPlaneRectangularProjection planeProjection;

        /// <summary>Transformation mode selected for this converter.</summary>
        public CoordinateTransformationMode Mode { get; }
        /// <summary>Fixed coordinate used as the local origin and height reference.</summary>
        public GpsCoordinate Origin => origin;
        /// <summary>Selected Japan zone. It is relevant only in <see cref="CoordinateTransformationMode.JapanPlaneRectangular"/> mode.</summary>
        public JapanPlaneRectangularZone JapanZone { get; }
        /// <summary>Selected datum for Japan Plane Rectangular mode.</summary>
        public JapanPlaneRectangularDatum JapanDatum { get; }

        /// <summary>Creates a converter with a fixed origin.</summary>
        public CoordinateConverter(GpsCoordinate origin, CoordinateTransformationMode mode = CoordinateTransformationMode.LocalEnu, JapanPlaneRectangularZone japanZone = JapanPlaneRectangularZone.IX, JapanPlaneRectangularDatum japanDatum = JapanPlaneRectangularDatum.Jgd2011Grs80)
        {
            if (mode != CoordinateTransformationMode.LocalEnu && mode != CoordinateTransformationMode.JapanPlaneRectangular)
                throw new ArgumentOutOfRangeException(nameof(mode));
            if ((int)japanZone < (int)JapanPlaneRectangularZone.I || (int)japanZone > (int)JapanPlaneRectangularZone.XIX)
                throw new ArgumentOutOfRangeException(nameof(japanZone));
            if (japanDatum != JapanPlaneRectangularDatum.Jgd2011Grs80 && japanDatum != JapanPlaneRectangularDatum.TokyoDatumBessel)
                throw new ArgumentOutOfRangeException(nameof(japanDatum));

            this.origin = origin;
            Mode = mode;
            JapanZone = japanZone;
            JapanDatum = japanDatum;
            ToEcef(origin, Wgs84SemiMajorAxis, Wgs84InverseFlattening, out originEcefX, out originEcefY, out originEcefZ);
            var latitude = origin.LatitudeDegrees * DegreesToRadians;
            var longitude = origin.LongitudeDegrees * DegreesToRadians;
            sinOriginLatitude = Math.Sin(latitude);
            cosOriginLatitude = Math.Cos(latitude);
            sinOriginLongitude = Math.Sin(longitude);
            cosOriginLongitude = Math.Cos(longitude);
            planeProjection = mode == CoordinateTransformationMode.JapanPlaneRectangular
                ? new JapanPlaneRectangularProjection(japanZone, japanDatum) : null;
        }

        /// <summary>Converts a GPS coordinate to Unity coordinates. Unity axes are X=east, Y=up, Z=north.</summary>
        public Vector3 ToUnity(GpsCoordinate coordinate)
        {
            if (Mode == CoordinateTransformationMode.JapanPlaneRectangular)
            {
                planeProjection.Project(coordinate, out var northing, out var easting);
                return new Vector3((float)easting, (float)coordinate.HeightMeters, (float)northing);
            }

            ToEcef(coordinate, Wgs84SemiMajorAxis, Wgs84InverseFlattening, out var x, out var y, out var z);
            var dx = x - originEcefX;
            var dy = y - originEcefY;
            var dz = z - originEcefZ;
            var east = -sinOriginLongitude * dx + cosOriginLongitude * dy;
            var north = -sinOriginLatitude * cosOriginLongitude * dx - sinOriginLatitude * sinOriginLongitude * dy + cosOriginLatitude * dz;
            var up = cosOriginLatitude * cosOriginLongitude * dx + cosOriginLatitude * sinOriginLongitude * dy + sinOriginLatitude * dz;
            return new Vector3((float)east, (float)up, (float)north);
        }

        private static void ToEcef(GpsCoordinate coordinate, double semiMajorAxis, double inverseFlattening, out double x, out double y, out double z)
        {
            var flattening = 1d / inverseFlattening;
            var eccentricitySquared = flattening * (2d - flattening);
            var latitude = coordinate.LatitudeDegrees * DegreesToRadians;
            var longitude = coordinate.LongitudeDegrees * DegreesToRadians;
            var sinLatitude = Math.Sin(latitude);
            var cosLatitude = Math.Cos(latitude);
            var radius = semiMajorAxis / Math.Sqrt(1d - eccentricitySquared * sinLatitude * sinLatitude);
            x = (radius + coordinate.HeightMeters) * cosLatitude * Math.Cos(longitude);
            y = (radius + coordinate.HeightMeters) * cosLatitude * Math.Sin(longitude);
            z = (radius * (1d - eccentricitySquared) + coordinate.HeightMeters) * sinLatitude;
        }

        private sealed class JapanPlaneRectangularProjection
        {
            private const double ScaleFactor = 0.9999d;
            private readonly double originLatitude;
            private readonly double originLongitude;
            private readonly double n;
            private readonly double aBar;
            private readonly double[] alpha;
            private readonly double xiOrigin;

            internal JapanPlaneRectangularProjection(JapanPlaneRectangularZone zone, JapanPlaneRectangularDatum datum)
            {
                GetZoneOrigin(zone, out originLatitude, out originLongitude);
                var semiMajorAxis = datum == JapanPlaneRectangularDatum.TokyoDatumBessel ? BesselSemiMajorAxis : Wgs84SemiMajorAxis;
                var inverseFlattening = datum == JapanPlaneRectangularDatum.TokyoDatumBessel ? BesselInverseFlattening : Grs80InverseFlattening;
                var flattening = 1d / inverseFlattening;
                n = flattening / (2d - flattening);
                var n2 = n * n;
                var n3 = n2 * n;
                var n4 = n2 * n2;
                var n5 = n4 * n;
                aBar = ScaleFactor * semiMajorAxis / (1d + n) * (1d + n2 / 4d + n4 / 64d);
                alpha = new[] {
                    0d,
                    0.5d * n - (2d / 3d) * n2 + (5d / 16d) * n3 + (41d / 180d) * n4 - (127d / 288d) * n5,
                    (13d / 48d) * n2 - (3d / 5d) * n3 + (557d / 1440d) * n4 + (281d / 630d) * n5,
                    (61d / 240d) * n3 - (103d / 140d) * n4 + (15061d / 26880d) * n5,
                    (49561d / 161280d) * n4 - (179d / 168d) * n5,
                    (34729d / 80640d) * n5
                };
                xiOrigin = ConformalLatitude(originLatitude * DegreesToRadians);
            }

            internal void Project(GpsCoordinate coordinate, out double northing, out double easting)
            {
                var latitude = coordinate.LatitudeDegrees * DegreesToRadians;
                var longitudeDifference = coordinate.LongitudeDegrees * DegreesToRadians - originLongitude;
                var t = Math.Sinh(Asinh(Math.Tan(latitude)) - 2d * Math.Sqrt(n) / (1d + n) * Atanh(2d * Math.Sqrt(n) / (1d + n) * Math.Sin(latitude)));
                var xiPrime = Math.Atan2(t, Math.Cos(longitudeDifference));
                var etaPrime = Atanh(Math.Sin(longitudeDifference) / Math.Sqrt(1d + t * t));
                var xi = xiPrime;
                var eta = etaPrime;
                for (var j = 1; j <= 5; j++)
                {
                    xi += alpha[j] * Math.Sin(2d * j * xiPrime) * Math.Cosh(2d * j * etaPrime);
                    eta += alpha[j] * Math.Cos(2d * j * xiPrime) * Math.Sinh(2d * j * etaPrime);
                }
                northing = aBar * (xi - xiOrigin);
                easting = aBar * eta;
            }

            private double ConformalLatitude(double latitude)
            {
                var t = Math.Sinh(Asinh(Math.Tan(latitude)) - 2d * Math.Sqrt(n) / (1d + n) * Atanh(2d * Math.Sqrt(n) / (1d + n) * Math.Sin(latitude)));
                var xiPrime = Math.Atan(t);
                var xi = xiPrime;
                for (var j = 1; j <= 5; j++) xi += alpha[j] * Math.Sin(2d * j * xiPrime);
                return xi;
            }

            private static double Atanh(double value) => 0.5d * Math.Log((1d + value) / (1d - value));

            private static double Asinh(double value) => Math.Log(value + Math.Sqrt(value * value + 1d));

            private static void GetZoneOrigin(JapanPlaneRectangularZone zone, out double latitudeDegrees, out double longitudeRadians)
            {
                double longitudeDegrees;
                switch (zone)
                {
                    case JapanPlaneRectangularZone.I: latitudeDegrees = 33d; longitudeDegrees = 129.5d; break;
                    case JapanPlaneRectangularZone.II: latitudeDegrees = 33d; longitudeDegrees = 131d; break;
                    case JapanPlaneRectangularZone.III: latitudeDegrees = 36d; longitudeDegrees = 132d + 10d / 60d; break;
                    case JapanPlaneRectangularZone.IV: latitudeDegrees = 33d; longitudeDegrees = 133.5d; break;
                    case JapanPlaneRectangularZone.V: latitudeDegrees = 36d; longitudeDegrees = 134d + 20d / 60d; break;
                    case JapanPlaneRectangularZone.VI: latitudeDegrees = 36d; longitudeDegrees = 136d; break;
                    case JapanPlaneRectangularZone.VII: latitudeDegrees = 36d; longitudeDegrees = 137d + 10d / 60d; break;
                    case JapanPlaneRectangularZone.VIII: latitudeDegrees = 36d; longitudeDegrees = 138.5d; break;
                    case JapanPlaneRectangularZone.IX: latitudeDegrees = 36d; longitudeDegrees = 139d + 50d / 60d; break;
                    case JapanPlaneRectangularZone.X: latitudeDegrees = 40d; longitudeDegrees = 140d + 50d / 60d; break;
                    case JapanPlaneRectangularZone.XI: latitudeDegrees = 44d; longitudeDegrees = 140d + 15d / 60d; break;
                    case JapanPlaneRectangularZone.XII: latitudeDegrees = 44d; longitudeDegrees = 142d + 15d / 60d; break;
                    case JapanPlaneRectangularZone.XIII: latitudeDegrees = 44d; longitudeDegrees = 144d + 15d / 60d; break;
                    case JapanPlaneRectangularZone.XIV: latitudeDegrees = 26d; longitudeDegrees = 142d; break;
                    case JapanPlaneRectangularZone.XV: latitudeDegrees = 26d; longitudeDegrees = 127.5d; break;
                    case JapanPlaneRectangularZone.XVI: latitudeDegrees = 26d; longitudeDegrees = 124d; break;
                    case JapanPlaneRectangularZone.XVII: latitudeDegrees = 26d; longitudeDegrees = 131d; break;
                    case JapanPlaneRectangularZone.XVIII: latitudeDegrees = 20d; longitudeDegrees = 136d; break;
                    case JapanPlaneRectangularZone.XIX: latitudeDegrees = 26d; longitudeDegrees = 154d; break;
                    default: throw new ArgumentOutOfRangeException(nameof(zone));
                }
                longitudeRadians = longitudeDegrees * DegreesToRadians;
            }
        }
    }
}
