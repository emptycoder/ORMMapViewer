using ORMMap;
using ORMMap.Model.Entitites;
using ORMMap.VectorTile.Geometry;
using ORMMapViewer.Model.Entitites;
using System.Collections.Generic;
using System.Linq;

namespace ORMMapViewer.Utils.Additional
{
	public static class RayCastMapTileUtils
	{
		public static List<string> GetMapTileHashes(this IEnumerable<RaycastMapResult> collection, int cZoom)
		{
			if (!collection.Any()) { return null; }

			// Find min and max latLng
			//LatLng minLatLng = new LatLng(double.MaxValue, double.MaxValue);
			//LatLng maxLatLng = new LatLng(double.MinValue, double.MinValue);

			//foreach (RaycastMapResult mapTile in collection)
			//{
			//	if (mapTile.LatLng.Lat < minLatLng.Lat)
			//	{
			//		minLatLng.Lat = mapTile.LatLng.Lat;
			//	}

			//	if (mapTile.LatLng.Lng < minLatLng.Lng)
			//	{
			//		minLatLng.Lng = mapTile.LatLng.Lng;
			//	}

			//	if (mapTile.LatLng.Lat > maxLatLng.Lat)
			//	{
			//		maxLatLng.Lat = mapTile.LatLng.Lat;
			//	}

			//	if (mapTile.LatLng.Lng > maxLatLng.Lng)
			//	{
			//		maxLatLng.Lng = mapTile.LatLng.Lng;
			//	}
			//}

			//List<string> hashes = new List<string>();
			//Vector2<double> minTileCoord = MercatorProjection.LatLngToTile(minLatLng, cZoom);
			//Vector3<double> vector3 = new Vector3<double>(minTileCoord.X, minTileCoord.Y, cZoom);
			// Vector2<int> maxTileCoord = MercatorProjection.LatLngToTile(maxLatLng, cZoom);

			// Hash generation
			/*for (double x = minTileCoord.X; x < maxTileCoord.X; x++)
			{
				for (double y = minTileCoord.Y; y < maxTileCoord.Y; y++)
				{
					hashes.Add(x + " " + y + " " + cZoom);
					
				}
			}*/

			//hashes.Add(vector3.ToString());
			//vector3 = new Vector3<double>(minTileCoord.X + 1, minTileCoord.Y + 1, cZoom);
			//hashes.Add(vector3.ToString());

			return null;
		}
	}
}
