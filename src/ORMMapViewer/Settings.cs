using ORMMap.VectorTile.Geometry;

namespace ORMMap
{
    public class Settings
    {
        // General
        // 19376, 24637
        public readonly static LatLng startPosition = new LatLng(48.464717f, 35.046183f);
        public const uint renderDistanceX = 2;
        public const uint renderDistanceY = 2;
        public const double zoom = 13;
        public const int cacheOpacity = 100;
    }
}
