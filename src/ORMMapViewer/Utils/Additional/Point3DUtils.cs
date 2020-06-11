using System;
using System.Windows.Media.Media3D;

namespace ORMMapViewer.Utils.Additional
{
	public static class Point3DUtils
	{
		public static bool InRange(this Point3D point, Point3D point1, int range)
		{
			return Math.Abs(point.X - point1.X) <= range && Math.Abs(point.Y - point1.Y) <= range;
		}
	}
}
