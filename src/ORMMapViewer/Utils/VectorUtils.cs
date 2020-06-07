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
		
		public static Vector2<int> Divide(this Vector2<int> vector, int number)
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
        
	}
}
