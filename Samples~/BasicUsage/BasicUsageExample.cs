using UnityEngine;

namespace ntk.GeospatialCoordinates.Samples
{
    /// <summary>Minimal component showing conversion into a target transform's local position.</summary>
    public sealed class BasicUsageExample : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private CoordinateConverterComponent converterComponent;

        private void Start()
        {
            if (target != null && converterComponent != null)
                target.localPosition = converterComponent.ConvertCurrentCoordinate();
        }
    }
}
