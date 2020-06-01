﻿using System;
using System.Collections.Generic;
using ORMMap.VectorTile.Contants;

namespace ORMMap.VectorTile.Geometry
{
	public static class DecodeGeometry
	{
		/// <summary>
		///     <para>returns a list of lists.</para>
		///     <para>If the root list contains one child list it is a single part feature</para>
		///     <para>and the child list contains the coordinate pairs.</para>
		///     <para>e.g. single part point:</para>
		///     <para> Parent list with one child list, child list contains one Pont2D</para>
		///     <para>If the root list contains several child lists, it is a multipart feature</para>
		///     <para>e.g. multipart or donut polygon:</para>
		///     <para> Parent list contains number of list equal to the number of parts.</para>
		///     <para> Each child list contains the corrdinates of this part.</para>
		/// </summary>
		/// <param name="extent">Tile extent</param>
		/// <param name="geomType">Geometry type</param>
		/// <param name="geometryCommands">VT geometry commands, see spec</param>
		/// <param name="scale">factor for scaling internal tile coordinates</param>
		/// <returns>List<List<Point2d<long>>>> of decoded geometries (in internal tile coordinates)</returns>
		public static List<List<Vector2<long>>> GetGeometry(
			ulong extent
			, GeomType geomType
			, List<uint> geometryCommands
			, float scale = 1.0f
		)
		{
			var geomOut = new List<List<Vector2<long>>>();
			var geomTmp = new List<Vector2<long>>();
			long cursorX = 0;
			long cursorY = 0;

			var geomCmdCnt = geometryCommands.Count;
			for (var i = 0; i < geomCmdCnt; i++)
			{
				var g = geometryCommands[i];
				var cmd = (Commands) (g & 0x7);
				var cmdCount = g >> 3;

				if (cmd == Commands.MoveTo || cmd == Commands.LineTo)
				{
					for (var j = 0; j < cmdCount; j++)
					{
						var delta = zigzagDecode(geometryCommands[i + 1], geometryCommands[i + 2]);
						cursorX += delta.X;
						cursorY += delta.Y;
						i += 2;
						//end of part of multipart feature
						if (cmd == Commands.MoveTo && geomTmp.Count > 0)
						{
							geomOut.Add(geomTmp);
							geomTmp = new List<Vector2<long>>();
						}

						//Point2d pntTmp = new Point2d(cursorX, cursorY);
						var pntTmp = new Vector2<long>
						{
							X = cursorX,
							Y = cursorY
						};
						geomTmp.Add(pntTmp);
					}
				}

				if (cmd == Commands.ClosePath)
					if (geomType == GeomType.POLYGON && geomTmp.Count > 0)
						geomTmp.Add(geomTmp[0]);
			}

			if (geomTmp.Count > 0) geomOut.Add(geomTmp);

			return geomOut;
		}


		/// <summary>
		/// </summary>
		/// <typeparam name="T">Type of <see cref="Vector2{T}" /> to be returned. Currently supported: int, long and float. </typeparam>
		/// <param name="inGeom">Geometry in internal tile coordinates.</param>
		/// <param name="scale">Scale factor.</param>
		/// <returns></returns>
		public static List<List<Vector2<T>>> Scale<T>(
			List<List<Vector2<long>>> inGeom
			, float scale = 1.0f
		)
		{
			var outGeom = new List<List<Vector2<T>>>();
			foreach (var inPart in inGeom)
			{
				var outPart = new List<Vector2<T>>();
				foreach (var inVertex in inPart)
				{
					var fX = inVertex.X * scale;
					var fY = inVertex.Y * scale;
					// TODO: find a better solution to make this work
					// scaled value has to be converted to target type beforehand
					// casting to T only works via intermediate cast to object
					// suppose (typeof(T) == typeof(x))
					// works         : T x = (T)(object)x; 
					// doesn't work  : T x = (T)x; 
					if (typeof(T) == typeof(int))
					{
						var x = Convert.ToInt32(fX);
						var y = Convert.ToInt32(fY);
						outPart.Add(new Vector2<T>((T) (object) x, (T) (object) y));
					}
					else if (typeof(T) == typeof(long))
					{
						var x = Convert.ToInt64(fX);
						var y = Convert.ToInt64(fY);
						outPart.Add(new Vector2<T>((T) (object) x, (T) (object) y));
					}
					else if (typeof(T) == typeof(float))
					{
						var x = Convert.ToSingle(fX);
						var y = Convert.ToSingle(fY);
						outPart.Add(new Vector2<T>((T) (object) x, (T) (object) y));
					}
				}

				outGeom.Add(outPart);
			}

			return outGeom;
		}

		private static Vector2<long> zigzagDecode(long x, long y)
		{
			//TODO: verify speed improvements using
			// new Point2d(){X=x, Y=y} instead of
			// new Point3d(x, y);

			//return new Point2d(
			//    ((x >> 1) ^ (-(x & 1))),
			//    ((y >> 1) ^ (-(y & 1)))
			//);
			return new Vector2<long>
			{
				X = (x >> 1) ^ -(x & 1),
				Y = (y >> 1) ^ -(y & 1)
			};
		}
	}
}
