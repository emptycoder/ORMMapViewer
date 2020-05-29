namespace ORMMap.Model.Entitites
{
    public struct Vector2<T> where T: struct
    {
        public readonly T X;
        public readonly T Y;

        public Vector2(T x, T y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}";
        }
    }
}
