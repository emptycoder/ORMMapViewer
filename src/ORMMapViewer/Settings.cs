using ORMMap.VectorTile.Geometry;

namespace ORMMap
{
	public class Settings
	{
		public const int zoom = 13;
		public const double zoomShowBuildingsAsModels = 18.5;

		public const int memoryDataCacheOpacity = 1000;
		public const int memoryMapTilesCacheOpacity = 1000;

		// Mask drawer
		public const int accuracy = 2;

		// General
		// 19376, 24637
		public static readonly LatLng startPosition = new LatLng(48.464717, 35.046183);
	}
}
