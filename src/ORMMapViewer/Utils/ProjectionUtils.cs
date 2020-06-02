using ORMMapViewer.Model.Entitites;
using System;

namespace ORMMapViewer.Utils
{
	public static class ProjectionUtils
	{
		// TODO: Fix angle usage
		public static Projection GetPerspectiveProjection(double fovH, double fovV, int zoom, int angleZ)
		{
			Projection projection = GetRectangleProjection(fovH, fovV, zoom);

			// pv - projection of vertical fov
			// pvs - small projection of vertical fov (trapezoid top where shift projection on angleZ)

			double side = GetPyramideSide(fovV, zoom);
			double pvs = projection.Left / side * Math.Sin(fovV);
			double height = (projection.Left * pvs) / 2 * Math.Tan(0.5 * Math.PI - (fovV / 2));
			// sqrt(height^2 + (bottom - height * ctg(90 deg - (fovV / 2))
			projection.Left = projection.Right = Math.Sqrt(Math.Pow(height, 2) + Math.Pow(projection.Bottom - (height / Math.Tan(0.5 * Math.PI - (fovV / 2))), 2));
			projection.Top = projection.Left * Math.Sin(Math.Acos(height / projection.Left));

			return projection;
		}

		public static Projection GetRectangleProjection(double fovH, double fovV, int zoom)
		{
			return new Projection(zoom * Math.Tan(fovH / 2), zoom * Math.Tan(fovV / 2));
		}

		private static double GetPyramideSide(double fovV, int zoom)
		{
			double height = zoom / Math.Cos(fovV / 2);
			return height / Math.Cos(fovV / 2);
		}
	}
}
