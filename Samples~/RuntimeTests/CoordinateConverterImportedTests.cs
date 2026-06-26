using NUnit.Framework;
using UnityEngine;

namespace ntk.GeospatialCoordinates.Tests
{
    /// <summary>Tests imported into Assets through the package sample workflow.</summary>
    public class CoordinateConverterImportedTests
    {
        private struct JapanZoneOrigin
        {
            public JapanPlaneRectangularZone Zone;
            public double LatitudeDegrees;
            public double LongitudeDegrees;

            public JapanZoneOrigin(JapanPlaneRectangularZone zone, double latitudeDegrees, double longitudeDegrees)
            {
                Zone = zone;
                LatitudeDegrees = latitudeDegrees;
                LongitudeDegrees = longitudeDegrees;
            }
        }

        private static readonly JapanZoneOrigin[] JapanZoneOrigins =
        {
            new JapanZoneOrigin(JapanPlaneRectangularZone.I, 33d, 129.5d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.II, 33d, 131d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.III, 36d, 132d + 10d / 60d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.IV, 33d, 133.5d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.V, 36d, 134d + 20d / 60d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.VI, 36d, 136d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.VII, 36d, 137d + 10d / 60d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.VIII, 36d, 138.5d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.IX, 36d, 139d + 50d / 60d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.X, 40d, 140d + 50d / 60d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.XI, 44d, 140d + 15d / 60d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.XII, 44d, 142d + 15d / 60d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.XIII, 44d, 144d + 15d / 60d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.XIV, 26d, 142d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.XV, 26d, 127.5d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.XVI, 26d, 124d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.XVII, 26d, 131d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.XVIII, 20d, 136d),
            new JapanZoneOrigin(JapanPlaneRectangularZone.XIX, 26d, 154d)
        };

        [Test]
        public void LocalEnu_OriginConvertsToZero()
        {
            var origin = new GpsCoordinate(35d, 139d, 42d);
            var result = new CoordinateConverter(origin).ToUnity(origin);
            Assert.That(result.x, Is.EqualTo(0f).Within(0.001f));
            Assert.That(result.y, Is.EqualTo(0f).Within(0.001f));
            Assert.That(result.z, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        public void LocalEnu_MapsEastUpNorthToUnityAxes()
        {
            var origin = new GpsCoordinate(35d, 139d, 0d);
            var converter = new CoordinateConverter(origin);
            var east = converter.ToUnity(new GpsCoordinate(35d, 139.001d, 0d));
            var north = converter.ToUnity(new GpsCoordinate(35.001d, 139d, 0d));
            var up = converter.ToUnity(new GpsCoordinate(35d, 139d, 10d));
            Assert.That(east.x, Is.GreaterThan(90f));
            Assert.That(Mathf.Abs(east.z), Is.LessThan(0.1f));
            Assert.That(north.z, Is.GreaterThan(100f));
            Assert.That(Mathf.Abs(north.x), Is.LessThan(0.1f));
            Assert.That(up.y, Is.EqualTo(10f).Within(0.001f));
        }

        [Test]
        public void JapanPlaneRectangular_ZoneOriginAndAbsoluteHeightAreMappedCorrectly()
        {
            var coordinate = new GpsCoordinate(36d, 139d + 50d / 60d, 25d);
            var converter = CoordinateConverter.CreateJapanPlaneRectangular(JapanPlaneRectangularZone.IX);
            var result = converter.ToUnity(coordinate);
            Assert.That(result.x, Is.EqualTo(0f).Within(0.001f));
            Assert.That(result.y, Is.EqualTo(25f).Within(0.001f));
            Assert.That(result.z, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        public void JapanPlaneRectangular_AllZoneOriginsAreZeroForEachDatum()
        {
            var datums = new[] { JapanPlaneRectangularDatum.Jgd2011Grs80, JapanPlaneRectangularDatum.TokyoDatumBessel };
            foreach (var zoneOrigin in JapanZoneOrigins)
            {
                var coordinate = new GpsCoordinate(zoneOrigin.LatitudeDegrees, zoneOrigin.LongitudeDegrees);
                foreach (var datum in datums)
                {
                    var result = CoordinateConverter.CreateJapanPlaneRectangular(zoneOrigin.Zone, datum).ToUnity(coordinate);
                    Assert.That(result.x, Is.EqualTo(0f).Within(0.001f), $"{zoneOrigin.Zone}, {datum}");
                    Assert.That(result.z, Is.EqualTo(0f).Within(0.001f), $"{zoneOrigin.Zone}, {datum}");
                }
            }
        }

        [Test]
        public void JapanPlaneRectangular_OriginFreeFactoryMatchesLegacyConstructor()
        {
            var coordinate = new GpsCoordinate(35.6813d, 139.7672d, 42d);
            var legacyOrigin = new GpsCoordinate(-80d, -170d, -100d);
            var originFree = CoordinateConverter.CreateJapanPlaneRectangular(JapanPlaneRectangularZone.IX).ToUnity(coordinate);
            var legacyConverter = new CoordinateConverter(legacyOrigin, CoordinateTransformationMode.JapanPlaneRectangular, JapanPlaneRectangularZone.IX);
            Assert.That(originFree, Is.EqualTo(legacyConverter.ToUnity(coordinate)));
            Assert.That(legacyConverter.Origin, Is.EqualTo(legacyOrigin));
        }

        [Test]
        public void JapanPlaneRectangular_AgreesWithZoneIxReferenceWithinOneMetre()
        {
            var coordinate = new GpsCoordinate(36d + 6d / 60d + 37.471d / 3600d, 140d + 5d / 60d + 25.040d / 3600d);
            var result = CoordinateConverter.CreateJapanPlaneRectangular(JapanPlaneRectangularZone.IX).ToUnity(coordinate);
            Assert.That(result.z, Is.EqualTo(12280.3f).Within(1f));
            Assert.That(result.x, Is.EqualTo(23133.4f).Within(1f));
        }

        [Test]
        public void JapanPlaneRectangular_TokyoDatumBesselAgreesWithGsiReference()
        {
            var coordinate = new GpsCoordinate(35.6813d, 139.7672d);
            var converter = CoordinateConverter.CreateJapanPlaneRectangular(JapanPlaneRectangularZone.IX, JapanPlaneRectangularDatum.TokyoDatumBessel);
            var result = converter.ToUnity(coordinate);
            Assert.That(result.z, Is.EqualTo(-35352.3883f).Within(0.01f));
            Assert.That(result.x, Is.EqualTo(-5985.4113f).Within(0.01f));
        }

        [Test]
        public void Component_ConvertsItsConfiguredOriginToZero()
        {
            var gameObject = new GameObject("Coordinate Converter Test");
            try
            {
                var component = gameObject.AddComponent<CoordinateConverterComponent>();
                component.SetCurrentCoordinate(component.Origin);
                component.ApplyCurrentCoordinate();
                Assert.That(gameObject.transform.localPosition.sqrMagnitude, Is.EqualTo(0f).Within(0.000001f));
            }
            finally
            {
                Object.DestroyImmediate(gameObject);
            }
        }
    }
}
