using NUnit.Framework;
using UnityEngine;

namespace ntk.GeospatialCoordinates.Tests
{
    public class CoordinateConverterTests
    {
        [Test]
        public void LocalEnu_OriginConvertsToZero()
        {
            var origin = new GpsCoordinate(35.0d, 139.0d, 42d);
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
            Assert.That(east.x, Is.GreaterThan(90f)); Assert.That(Mathf.Abs(east.z), Is.LessThan(0.1f));
            Assert.That(north.z, Is.GreaterThan(100f)); Assert.That(Mathf.Abs(north.x), Is.LessThan(0.1f));
            Assert.That(up.y, Is.EqualTo(10f).Within(0.001f));
        }

        [Test]
        public void JapanPlaneRectangular_ZoneOriginIsZeroAndHeightIsRelative()
        {
            var origin = new GpsCoordinate(36d, 139d + 50d / 60d, 12d);
            var converter = new CoordinateConverter(origin, CoordinateTransformationMode.JapanPlaneRectangular, JapanPlaneRectangularZone.IX);
            var result = converter.ToUnity(new GpsCoordinate(origin.LatitudeDegrees, origin.LongitudeDegrees, 25d));
            Assert.That(result.x, Is.EqualTo(0f).Within(0.001f));
            Assert.That(result.z, Is.EqualTo(0f).Within(0.001f));
            Assert.That(result.y, Is.EqualTo(13f).Within(0.001f));
        }

        [Test]
        public void JapanPlaneRectangular_AgreesWithGsiPublishedExampleWithinOneMetre()
        {
            // GSI forward-projection example: zone IX, 36°06′37.471″N, 140°05′25.040″E.
            var coordinate = new GpsCoordinate(36d + 6d / 60d + 37.471d / 3600d, 140d + 5d / 60d + 25.040d / 3600d);
            var converter = new CoordinateConverter(new GpsCoordinate(36d, 139d + 50d / 60d), CoordinateTransformationMode.JapanPlaneRectangular, JapanPlaneRectangularZone.IX);
            var result = converter.ToUnity(coordinate);
            Assert.That(result.z, Is.EqualTo(12280.3f).Within(1f));
            Assert.That(result.x, Is.EqualTo(23133.4f).Within(1f));
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
