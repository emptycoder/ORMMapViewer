namespace ORMMap.Model.Entitites
{
    public struct Vector3<T> where T : struct
    {
        public readonly T X;
        public readonly T Y;
        public readonly T Z;
        public Vector3(T x, T y, T z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
