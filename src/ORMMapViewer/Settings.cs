using ORMMap.VectorTile.Geometry;

namespace ORMMap
{
	public class Settings
	{
		public const int zoom = 13;

		public const int memoryDataCacheOpacity = 100;
		public const int memoryMapModelsCacheOpacity = 50;

		// General
		// 19376, 24637
		public static readonly LatLng startPosition = new LatLng(48.464717f, 35.046183f);
	}
}
