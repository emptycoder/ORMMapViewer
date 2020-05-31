using ORMMap.VectorTile.Geometry;
using System;

namespace ORMMap
{
    class MercatorProjection
    {
        private double halfCircumferenceMeters = 20037508.342789244;
        private double circumferenceMeters;

        // 256 is the size of a tile in google image
        public MercatorProjection(uint tileSize, uint tileScale)
        {
            this.circumferenceMeters = halfCircumferenceMeters * 2;
            
            // this.radius = (this.circumference) / (2 * Math.PI);
            // this.center = new Vector2<double>(this.circumference / 2, this.circumference / 2);
        }

        private LatLng MetersToLatLng(Vector2<double> meters)
        {
            meters.X /= halfCircumferenceMeters;
            meters.Y /= halfCircumferenceMeters;

            meters.Y = (2 * Math.Atan(Math.Exp(meters.Y * Math.PI)) - Math.PI / 2) / Math.PI;

            meters.X *= 180;
            meters.Y *= 180;

            return new LatLng(meters.X, meters.Y);
        }

        private Vector2<double> LatLngToMeters(LatLng latLng)
        {
            double y = Math.Log(Math.Tan(latLng.Lat * Math.PI / 360 + Math.PI / 4), Math.E) / Math.PI;
            y *= halfCircumferenceMeters;

            double x = latLng.Lng * halfCircumferenceMeters / 180;

            return new Vector2<double>(x, y);
        }

        public LatLng TileToLatLng(Vector2<uint> tile, double zoom)
        {
            double x = tile.X * circumferenceMeters / Math.Pow(2, zoom) - halfCircumferenceMeters;
            double y = -(tile.Y * circumferenceMeters / Math.Pow(2, zoom) - halfCircumferenceMeters);

            return MetersToLatLng(new Vector2<double>(x, y));
        }

        public Vector2<uint> LatLngToTile(LatLng latLng, double zoom)
        {
            Vector2<double> tile = LatLngToMeters(latLng);
            
            double x = Math.Floor((tile.X + halfCircumferenceMeters) / (circumferenceMeters / Math.Pow(2, zoom)));
            double y = Math.Floor((-tile.Y + halfCircumferenceMeters) / (circumferenceMeters / Math.Pow(2, zoom)));

            return new Vector2<uint>((uint)x, (uint)y);
        }

        public double getLongitudeFromX(double xValue, double zoom)
        {
            double circumference = 4096 * Math.Pow(2, zoom);
            double radius = (circumference) / (2 * Math.PI);
            Vector2<double> center = new Vector2<double>(circumference / 2, circumference / 2);
            xValue -= center.X;
            double longitude = xValue / radius;
            longitude = longitude * 180 / Math.PI;

            return longitude;
        }

        public double getLatitudeFromY(double yValue, double zoom)
        {
            double circumference = 4096 * Math.Pow(2, zoom);
            double radius = (circumference) / (2 * Math.PI);
            Vector2<double> center = new Vector2<double>(circumference / 2, circumference / 2);
            yValue = center.Y - yValue;

            double InvLog = yValue / (radius * 0.5);
            InvLog = Math.Pow(Math.E, InvLog);

            double latitude = Math.Asin((InvLog - 1) / (InvLog + 1));
            latitude = latitude * 180 / Math.PI;

            return latitude;
        }

        public double getXFromLongitude(double longInDegrees, double zoom)
        {
            double circumference = 4096 * Math.Pow(2, zoom);
            double radius = (circumference) / (2 * Math.PI);
            Vector2<double> center = new Vector2<double>(circumference / 2, circumference / 2);
            double longInRadians = longInDegrees * Math.PI / 180;
            double x = radius * longInRadians;
            x = center.X + x;

            return x;
        }

        public double getYFromLatitude(double latInDegrees, double zoom)
        {
            double circumference = 4096 * Math.Pow(2, zoom);
            double radius = (circumference) / (2 * Math.PI);
            Vector2<double> center = new Vector2<double>(circumference / 2, circumference / 2);
            double latInRadians = latInDegrees * Math.PI / 180;
            double logVal = Math.Log(((1 + Math.Sin(latInRadians)) / (1 - Math.Sin(latInRadians))), Math.E);

            double y = radius * 0.5 * logVal;
            y = center.Y - y;

            return y;
        }
    }
}
