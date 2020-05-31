using ORMMap.VectorTile.Geometry;
using System;

namespace ORMMap
{
    class MercatorProjection
    {
        private double halfCircumferenceMeters = 20037508.342789244;
        private double circumferenceMeters;

        private uint tileSize;
        private uint tileScale;

        // 256 is the size of a tile in google image
        public MercatorProjection(uint tileSize, uint tileScale)
        {
            this.tileSize = tileSize;
            this.tileScale = tileScale;
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

        public LatLng TileToLatLng(Vector2<uint> tile, uint zoom)
        {
            double x = tile.X * circumferenceMeters / Math.Pow(2, zoom) - halfCircumferenceMeters;
            double y = -(tile.Y * circumferenceMeters / Math.Pow(2, zoom) - halfCircumferenceMeters);

            return MetersToLatLng(new Vector2<double>(x, y));
        }

        public Vector2<uint> LatLngToTile(LatLng latLng, uint zoom)
        {
            Vector2<double> tile = LatLngToMeters(latLng);
            
            double x = Math.Floor((tile.X + halfCircumferenceMeters) / (circumferenceMeters / Math.Pow(2, zoom)));
            double y = Math.Floor((-tile.Y + halfCircumferenceMeters) / (circumferenceMeters / Math.Pow(2, zoom)));

            return new Vector2<uint>((uint)x, (uint)y);
        }

        private double GetUnitsPerMeter(uint zoom)
        {
            double metersPerPixel = this.circumferenceMeters / tileSize / Math.Pow(2, zoom);
            return tileScale / (tileSize * metersPerPixel);
        }
    }
}
