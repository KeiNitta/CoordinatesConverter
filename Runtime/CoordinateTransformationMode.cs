namespace ntk.GeospatialCoordinates
{
    /// <summary>Coordinate transformation used by <see cref="CoordinateConverter"/>.</summary>
    public enum CoordinateTransformationMode
    {
        /// <summary>WGS84 ECEF delta transformed to local east, up, north axes.</summary>
        LocalEnu = 0,
        /// <summary>Japan Plane Rectangular Coordinate System using the selected Japan Plane Rectangular datum.</summary>
        JapanPlaneRectangular = 1
    }
}
