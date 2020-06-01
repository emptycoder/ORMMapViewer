using System;
using ORMMap.VectorTile.Geometry;

namespace ORMMap
{
	public static class MercatorProjection
	{
		private static readonly double halfCircumferenceMeters = 20037508.342789244;
		private static readonly double circumferenceMeters = halfCircumferenceMeters * 2;

		private static LatLng MetersToLatLng(Vector2<double> meters)
		{
			meters.X /= halfCircumferenceMeters;
			meters.Y /= halfCircumferenceMeters;

			meters.Y = (2 * Math.Atan(Math.Exp(meters.Y * Math.PI)) - Math.PI / 2) / Math.PI;

			meters.X *= 180;
			meters.Y *= 180;

			return new LatLng(meters.X, meters.Y);
		}

		private static Vector2<double> LatLngToMeters(LatLng latLng)
		{
			double y = Math.Log(Math.Tan(latLng.Lat * Math.PI / 360 + Math.PI / 4), Math.E) / Math.PI;
			y *= halfCircumferenceMeters;

			double x = latLng.Lng * halfCircumferenceMeters / 180;

			return new Vector2<double>(x, y);
		}

		public static LatLng TileToLatLng(Vector2<uint> tile, double zoom)
		{
			double x = tile.X * circumferenceMeters / Math.Pow(2, zoom) - halfCircumferenceMeters;
			double y = -(tile.Y * circumferenceMeters / Math.Pow(2, zoom) - halfCircumferenceMeters);

			return MetersToLatLng(new Vector2<double>(x, y));
		}

		public static Vector2<uint> LatLngToTile(LatLng latLng, double zoom)
		{
			Vector2<double> tile = LatLngToMeters(latLng);

			double x = Math.Floor((tile.X + halfCircumferenceMeters) / (circumferenceMeters / Math.Pow(2, zoom)));
			double y = Math.Floor((-tile.Y + halfCircumferenceMeters) / (circumferenceMeters / Math.Pow(2, zoom)));

			return new Vector2<uint>((uint) x, (uint) y);
		}
	}
}
