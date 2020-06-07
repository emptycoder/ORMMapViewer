using ORMMapViewer.Model.Entitites;
using System;

namespace ORMMapViewer.Utils
{
	public static class ProjectionUtils
	{
		/// <summary>
		/// Calculate camera view projection on scene with perspective.
		/// </summary>
		/// <param name="fovH">Horizontal FOV in radians.</param>
		/// <param name="fovV">Vertical FOV in radians.</param>
		/// <param name="zoom">Distance to scene. Height from eyepoint to scene.</param>
		/// <param name="angleZ">Angle of perspective.</param>
		/// <returns>Camera view projection.</returns>
		public static Projection GetPerspectiveProjection(double fovH, double fovV, int zoom, double angleZ)
		{
			Projection projection = GetRectangleProjection(fovH, fovV, zoom);

			// pv - projection of vertical fov = projection.Left = projection.Right
			// ph - projection of horizontal fov (for rectangle: = projection.Top = projection.Bottom)
			// pvs - small projection of vertical fov (trapezoid top where shift projection on angleZ)

			double side = GetPyramideSide(fovV, zoom);
			double pvs = projection.Left / side * Math.Sin(fovV / 2);
			double height = (projection.Left - (pvs * 2)) / 2 * Math.Tan(angleZ);
			// sqrt(height^2 + (bottom - height * ctg(90 deg - (fovV / 2))
			projection.Left = projection.Right = Math.Sqrt(Math.Pow(height, 2) + Math.Pow(projection.Left - (height / Math.Tan(0.5 * Math.PI - (fovV / 2))), 2));
			double underZoom = zoom - (height * Math.Cos(fovH / 2));
			projection.Top = underZoom * Math.Tan(fovH / 2);

			return projection;
		}

		/// <summary>
		/// Calculate camera view projection on scene.
		/// </summary>
		/// <param name="fovH">Horizontal FOV in radians.</param>
		/// <param name="fovV">Vertical FOV in radians.</param>
		/// <param name="zoom">Distance to scene. Height from eyepoint to scene.</param>
		/// <returns>Camera view projection.</returns>
		public static Projection GetRectangleProjection(double fovH, double fovV, int zoom)
		{
			return new Projection(zoom * Math.Tan(fovH / 2) * 2, zoom * Math.Tan(fovV / 2) * 2);
		}

		/// <summary>
		/// Calculate fovV from aspect ratio.
		/// </summary>
		/// <param name="fovH">Horizontal FOV in radians.</param>
		/// <param name="aspect">Scene.width / scene.height.</param>
		/// <returns>Vertical FOV in radians.</returns>
		public static double FovVFromAspectRatio(double fovH, double aspect)
		{
			return fovH / aspect;
		}

		/// <summary>
		/// Get side of pyramide that connect eyepoint with projection corner points.
		/// </summary>
		/// <param name="fovV">Vertical FOV in radians.</param>
		/// <param name="zoom">Distance to scene. Height from eyepoint to scene.</param>
		/// <returns>Length of pyramide side.</returns>
		private static double GetPyramideSide(double fovV, int zoom)
		{
			return zoom / (2 * Math.Cos(fovV / 2));
		}
	}
}
