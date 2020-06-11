using ORMMap.Model.Entitites;
using ORMMap.VectorTile.Geometry;
using System;

namespace ORMMapViewer.Utils
{
	public static class VectorUtils
	{
		public static double GetDistance(this Vector2<int> vector1, Vector2<int> vector2)
		{
			return Math.Sqrt(Math.Pow(vector1.X - vector2.X, 2) + Math.Pow(vector1.Y - vector2.Y, 2));
		}

        public static double GetDistance(this Vector2<int> vector1, Vector2<double> vector2)
        {
            return Math.Sqrt(Math.Pow(vector1.X - vector2.X, 2) + Math.Pow(vector1.Y - vector2.Y, 2));
        }

        public static double GetDistance(this Vector2<int> vector1, int x, int y)
        {
            return Math.Sqrt(Math.Pow(vector1.X - x, 2) + Math.Pow(vector1.Y - y, 2));
        }

        public static bool IsEquals(this Vector3<double> vector, Vector3<double> vector1)
        {
            return vector.X == vector1.X && vector.Y == vector1.Y && vector.Z == vector1.Z;
        }

        public static bool IsEquals(this Vector2<int> vector, Vector2<int> vector1)
        {
            return vector.X == vector1.X && vector.Y == vector1.Y;
        }

        public static bool InRange(this Vector2<int> vector, Vector2<int> vector1, double range)
        {
            return Math.Abs(vector.X - vector1.X) <= range && Math.Abs(vector.Y - vector1.Y) <= range;
        }

        public static bool InRange(this Vector2<double> vector, Vector2<double> vector1, double range)
        {
            return Math.Abs(vector.X - vector1.X) <= range && Math.Abs(vector.Y - vector1.Y) <= range;
        }

        public static bool InRange(this Vector2<int> vector, Vector2<int> vector1, Vector2<int> range)
        {
            return Math.Abs(vector.X - vector1.X) <= range.X && Math.Abs(vector.Y - vector1.Y) <= range.Y;
        }

        public static bool InMinMax(this Vector2<int> vector, double min, double max)
        {
            return vector.X >= min && vector.X <= max && vector.Y >= min && vector.Y <= max;
        }

        public static Vector2<int> Divide(this Vector2<int> vector, int number)
        {
            vector.X /= number;
            vector.Y /= number;
            
            return vector;
        }

        public static Vector2<double> Divide(this Vector2<double> vector, int number)
        {
            vector.X /= number;
            vector.Y /= number;

            return vector;
        }

        public static Vector2<int> Multiply(this Vector2<int> vector, int number)
        {
            vector.X *= number;
            vector.Y *= number;
            
            return vector;
        }

        public static Vector2<double> Multiply(this Vector2<double> vector, int number)
        {
            vector.X *= number;
            vector.Y *= number;

            return vector;
        }

        public static Vector2<int> Add(this Vector2<int> vector, int number)
        {
            vector.X += number;
            vector.Y += number;
            
            return vector;
        }

        public static Vector2<int> Add(this Vector2<int> vector, Vector2<int> other)
        {
            vector.X += other.X;
            vector.Y += other.Y;

            return vector;
        }

        public static Vector2<int> Add(this Vector2<int> vector, int x, int y)
        {
            vector.X += x;
            vector.Y += y;

            return vector;
        }

        public static Vector2<int> Substract(this Vector2<int> vector, int number)
        {
            vector.X -= number;
            vector.Y -= number;
            
            return vector;
        }
        
        public static Vector2<int> Substract(this Vector2<int> vector, Vector2<int> other)
        {
            vector.X -= other.X;
            vector.Y -= other.Y;
            
            return vector;
        }

        public static Vector2<double> Substract(this Vector2<double> vector, int number)
        {
            vector.X -= number;
            vector.Y -= number;

            return vector;
        }

        public static Vector2<double> Substract(this Vector2<double> vector, Vector2<double> other)
        {
            vector.X -= other.X;
            vector.Y -= other.Y;

            return vector;
        }

        public static Vector2<double> SubstractionBalance(this Vector2<double> vector, double value)
        {
            while (vector.X >= value)
            {
                vector.X -= value;
            }

            while (vector.Y >= value)
            {
                vector.Y -= value;
            }

            return vector;
        }

        public static Vector2<double> Floor(this Vector2<double> vector)
        {
            vector.X = Math.Floor(vector.X);
            vector.Y = Math.Floor(vector.Y);

            return vector;
        }

        public static Vector2<int> ToVectorInt(this Vector2<double> vector)
        {
            return new Vector2<int>((int)Math.Floor(vector.X), (int)Math.Floor(vector.Y));
        }

        public static Vector2<double> Clone(this Vector2<double> vector)
		{
            return new Vector2<double>(vector.X, vector.Y);
		}
    }
}
