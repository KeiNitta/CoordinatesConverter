namespace ntk.GeospatialCoordinates
{
    /// <summary>Geodetic datum and ellipsoid used for Japan Plane Rectangular projection.</summary>
    public enum JapanPlaneRectangularDatum
    {
        /// <summary>JGD2011-compatible GRS80 ellipsoid. Use for normal WGS84/GNSS input.</summary>
        Jgd2011Grs80 = 0,

        /// <summary>Legacy Japanese geodetic datum (Tokyo Datum) using the Bessel ellipsoid.</summary>
        TokyoDatumBessel = 1
    }
}
