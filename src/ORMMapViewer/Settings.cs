using ORMMap.Model.Entitites;

namespace ORMMap
{
    public class Settings
    {
        // General
        public readonly static Vector2<uint> startPosition = new Vector2<uint>(19376, 24637);
        public const uint renderDistanceX = 2;
        public const uint renderDistanceY = 2;
        public const uint zoom = 16;
        public const int cacheOpacity = 100;
    }
}
