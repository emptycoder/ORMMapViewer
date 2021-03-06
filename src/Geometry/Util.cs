﻿using System.Collections.Generic;
using ORMMap.VectorTile.Geometry.InteralClipperLib;

namespace ORMMap.VectorTile.Geometry
{
	using Polygon = List<InternalClipper.IntPoint>;
	using Polygons = List<List<InternalClipper.IntPoint>>;

	public static class UtilGeom
	{
		/// <summary>
		///     TO BE REMOVED!!! Processing geometries is out of scope.
		///     Clip geometries extending beyond the tile border.
		/// </summary>
		/// <param name="geoms">Raw tile geometries of the feature</param>
		/// <param name="geomType">Geometry type of the feature</param>
		/// <param name="extent">Extent of the layer </param>
		/// <param name="bufferSize">
		///     Units (in internal tile coordinates) to go beyond the tile border. Pass '0' to clip exactly at
		///     the tile border
		/// </param>
		/// <param name="scale">Factor for scaling the geometries</param>
		/// <returns></returns>
		public static List<List<Vector2<long>>> ClipGeometries(
			List<List<Vector2<long>>> geoms
			, GeomType geomType
			, long extent
			, uint bufferSize
			, float scale
		)
		{
			var retVal = new List<List<Vector2<long>>>();

			//points: simply remove them if one part of the coordinate pair is out of bounds:
			// <0 || >extent
			if (geomType == GeomType.POINT)
			{
				foreach (List<Vector2<long>> geomPart in geoms)
				{
					var outGeom = new List<Vector2<long>>();
					foreach (Vector2<long> geom in geomPart)
					{
						if (
							geom.X < 0L - bufferSize
							|| geom.Y < 0L - bufferSize
							|| geom.X > extent + bufferSize
							|| geom.Y > extent + bufferSize
						)
						{
							continue;
						}

						outGeom.Add(geom);
					}

					if (outGeom.Count > 0)
					{
						retVal.Add(outGeom);
					}
				}

				return retVal;
			}

			//use clipper for lines and polygons
			bool closed = true;
			if (geomType == GeomType.LINESTRING)
			{
				closed = false;
			}


			var subjects = new Polygons();
			var clip = new Polygons(1);
			var solution = new Polygons();

			clip.Add(new Polygon(4));
			clip[0].Add(new InternalClipper.IntPoint(0L - bufferSize, 0L - bufferSize));
			clip[0].Add(new InternalClipper.IntPoint(extent + bufferSize, 0L - bufferSize));
			clip[0].Add(new InternalClipper.IntPoint(extent + bufferSize, extent + bufferSize));
			clip[0].Add(new InternalClipper.IntPoint(0L - bufferSize, extent + bufferSize));

			foreach (List<Vector2<long>> geompart in geoms)
			{
				var part = new Polygon();

				foreach (Vector2<long> geom in geompart)
				{
					part.Add(new InternalClipper.IntPoint(geom.X, geom.Y));
				}

				subjects.Add(part);
			}

			InternalClipper.Clipper c = new InternalClipper.Clipper();
			c.AddPaths(subjects, InternalClipper.PolyType.ptSubject, closed);
			c.AddPaths(clip, InternalClipper.PolyType.ptClip, true);

			bool succeeded = false;
			if (geomType == GeomType.LINESTRING)
			{
				InternalClipper.PolyTree lineSolution = new InternalClipper.PolyTree();
				succeeded = c.Execute(
					InternalClipper.ClipType.ctIntersection
					, lineSolution
					, InternalClipper.PolyFillType.pftNonZero
					, InternalClipper.PolyFillType.pftNonZero
				);
				if (succeeded)
				{
					solution = InternalClipper.Clipper.PolyTreeToPaths(lineSolution);
				}
			}
			else
			{
				succeeded = c.Execute(
					InternalClipper.ClipType.ctIntersection
					, solution
					, InternalClipper.PolyFillType.pftNonZero
					, InternalClipper.PolyFillType.pftNonZero
				);
			}

			if (succeeded)
			{
				retVal = new List<List<Vector2<long>>>();
				foreach (Polygon part in solution)
				{
					var geompart = new List<Vector2<long>>();
					// HACK:
					// 1. clipper may or may not reverse order of vertices of LineStrings
					// 2. clipper semms to drop the first vertex of a Polygon
					// * We don't care about 1.
					// * Added a check for 2 and insert a copy of last vertex as first
					foreach (InternalClipper.IntPoint geom in part)
					{
						geompart.Add(new Vector2<long> {X = geom.X, Y = geom.Y});
					}

					if (geomType == GeomType.POLYGON)
					{
						if (!geompart[0].Equals(geompart[geompart.Count - 1]))
						{
							geompart.Insert(0, geompart[geompart.Count - 1]);
						}
					}

					retVal.Add(geompart);
				}

				return retVal;
			}

			//if clipper was not successfull return original geometries
			return geoms;
		}
	}
}
