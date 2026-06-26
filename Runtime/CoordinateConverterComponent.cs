using UnityEngine;

namespace ntk.GeospatialCoordinates
{
    /// <summary>
    /// Unity component wrapper for <see cref="CoordinateConverter"/>.
    /// Configure the origin and mode in the Inspector, then call <see cref="ToUnity"/> from game code.
    /// </summary>
    [AddComponentMenu("Geospatial Coordinates/Coordinate Converter")]
    public sealed class CoordinateConverterComponent : MonoBehaviour
    {
        [Header("Origin (WGS84 degrees / ellipsoidal metres)")]
        [SerializeField] private double originLatitudeDegrees = 35.681236d;
        [SerializeField] private double originLongitudeDegrees = 139.767125d;
        [SerializeField] private double originHeightMeters;

        [Header("Transformation")]
        [SerializeField] private CoordinateTransformationMode mode = CoordinateTransformationMode.LocalEnu;
        [Tooltip("Used only when Mode is Japan Plane Rectangular.")]
        [SerializeField] private JapanPlaneRectangularZone japanZone = JapanPlaneRectangularZone.IX;

        [Header("Current GPS coordinate (WGS84 degrees / ellipsoidal metres)")]
        [SerializeField] private double currentLatitudeDegrees = 35.681300d;
        [SerializeField] private double currentLongitudeDegrees = 139.767200d;
        [SerializeField] private double currentHeightMeters;

        [Header("Optional target")]
        [SerializeField] private Transform target;
        [SerializeField] private bool applyCurrentCoordinateOnStart = true;

        private CoordinateConverter converter;

        /// <summary>Configured transformation mode.</summary>
        public CoordinateTransformationMode Mode => mode;

        /// <summary>Configured Japan Plane Rectangular zone. It is ignored in Local ENU mode.</summary>
        public JapanPlaneRectangularZone JapanZone => japanZone;

        /// <summary>Configured fixed GPS origin. Height is used only in Local ENU mode.</summary>
        public GpsCoordinate Origin => new GpsCoordinate(originLatitudeDegrees, originLongitudeDegrees, originHeightMeters);

        /// <summary>Current GPS coordinate configured in the Inspector or with <see cref="SetCurrentCoordinate"/>.</summary>
        public GpsCoordinate CurrentCoordinate => new GpsCoordinate(currentLatitudeDegrees, currentLongitudeDegrees, currentHeightMeters);

        private void Awake()
        {
            Rebuild();
        }

        private void Start()
        {
            if (applyCurrentCoordinateOnStart)
                ApplyCurrentCoordinate();
        }

        /// <summary>Rebuilds the converter after changing Inspector or configuration values.</summary>
        public void Rebuild()
        {
            converter = new CoordinateConverter(Origin, mode, japanZone);
        }

        /// <summary>Converts a GPS coordinate using the current component configuration.</summary>
        public Vector3 ToUnity(GpsCoordinate coordinate)
        {
            if (converter == null)
                Rebuild();

            return converter.ToUnity(coordinate);
        }

        /// <summary>Converts degree/metre GPS values using the current component configuration.</summary>
        public Vector3 ToUnity(double latitudeDegrees, double longitudeDegrees, double heightMeters = 0d)
        {
            return ToUnity(new GpsCoordinate(latitudeDegrees, longitudeDegrees, heightMeters));
        }

        /// <summary>Updates the current GPS coordinate displayed in the Inspector.</summary>
        public void SetCurrentCoordinate(GpsCoordinate coordinate)
        {
            currentLatitudeDegrees = coordinate.LatitudeDegrees;
            currentLongitudeDegrees = coordinate.LongitudeDegrees;
            currentHeightMeters = coordinate.HeightMeters;
        }

        /// <summary>Converts the configured current coordinate.</summary>
        public Vector3 ConvertCurrentCoordinate()
        {
            return ToUnity(CurrentCoordinate);
        }

        /// <summary>Applies the converted current coordinate to the assigned target, or this GameObject when no target is assigned.</summary>
        public void ApplyCurrentCoordinate()
        {
            var destination = target != null ? target : transform;
            destination.localPosition = ConvertCurrentCoordinate();
        }
    }
}
