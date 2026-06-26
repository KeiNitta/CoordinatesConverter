using UnityEngine;

namespace ntk.GeospatialCoordinates.Samples
{
    /// <summary>Minimal component showing conversion into a target transform's local position.</summary>
    public sealed class BasicUsageExample : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private void Start()
        {
            var origin = new GpsCoordinate(35.681236d, 139.767125d, 40d);
            var converter = new CoordinateConverter(origin, CoordinateTransformationMode.LocalEnu);
            var measuredPoint = new GpsCoordinate(35.681300d, 139.767200d, 42d);

            if (target != null)
                target.localPosition = converter.ToUnity(measuredPoint);
        }
    }
}
