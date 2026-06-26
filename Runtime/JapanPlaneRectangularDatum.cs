namespace ntk.GeospatialCoordinates
{
    /// <summary>Geodetic datum and ellipsoid used for Japan Plane Rectangular projection.</summary>
    public enum JapanPlaneRectangularDatum
    {
        /// <summary>JGD2011-compatible GRS80 ellipsoid. Use WGS84/GNSS latitude and longitude; no datum transformation is performed.</summary>
        Jgd2011Grs80 = 0,

        /// <summary>Legacy Japanese geodetic datum (Tokyo Datum) using the Bessel ellipsoid. Input latitude and longitude must already be Tokyo Datum values.</summary>
        TokyoDatumBessel = 1
    }
}
