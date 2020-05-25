using ORMMap.Model.Entitites;
using System;

namespace ORMMap
{
    class MercatorProjection
    {
        public uint Zoom { get; }
        private double circumference;
        private double radius;
        private Vector2<double> center;

        // 256 is the size of a tile in google image
        public MercatorProjection(uint tileSize, uint zoom)
        {
            this.Zoom = zoom;
            this.circumference = tileSize * Math.Pow(2, zoom);
            this.radius = (this.circumference) / (2 * Math.PI);
            this.center = new Vector2<double>(this.circumference / 2, this.circumference / 2);
        }

        public double getXFromLongitude(double longInDegrees)
        {
            double longInRadians = longInDegrees * Math.PI / 180;
            double x = this.radius * longInRadians;
            x = this.center.X + x;

            return x;
        }

        public double getLongitudeFromX(double xValue)
        {
            xValue = xValue - this.center.X;
            double longitude = xValue / this.radius;
            longitude = longitude * 180 / Math.PI;

            return longitude;
        }

        public double getYFromLatitude(double latInDegrees)
        {
            double latInRadians = latInDegrees * Math.PI / 180;
            double logVal = Math.Log(((1 + Math.Sin(latInRadians)) / (1 - Math.Sin(latInRadians))), Math.E);

            double y = this.radius * 0.5 * logVal;
            y = this.center.Y - y;

            return y;
        }
        public double getLatitudeFromY(double yValue)
        {
            yValue = this.center.Y - yValue;

            double InvLog = yValue / (this.radius * 0.5);
            InvLog = Math.Pow(Math.E, InvLog);

            double latitude = Math.Asin((InvLog - 1) / (InvLog + 1));
            latitude = latitude * 180 / Math.PI;

            return latitude;
        }
    }
}
