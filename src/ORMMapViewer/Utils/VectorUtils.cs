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
	}
}
