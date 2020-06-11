using System;
using ORMMap.VectorTile.Geometry;
using ORMMapViewer.Utils;

namespace ORMMapViewer.Model.Entitites
{
	public class Segment
	{
		public static readonly Vector2<int> NotIntersected = new Vector2<int>(int.MinValue, int.MinValue);
		public readonly int A, B, C;
		public Node startNode;
		public Node endNode;

		public Segment(Node first, Node second)
		{
			startNode = first;
			endNode = second;

			A = second.pos.Y - first.pos.Y;
			B = first.pos.X - second.pos.X;
			C = A * first.pos.X + B * first.pos.Y;
		}

		// Only for calculations
		public Segment(Vector2<int> startPos, Vector2<int> endPos)
		{
			A = endPos.Y - startPos.Y;
			B = startPos.X - endPos.X;
			C = A * startPos.X + B * startPos.Y;
		}

		public Vector2<int> Intersection(Segment other)
		{
			int det = A * other.B - other.A * B;

			if (det == 0)
			{
				return NotIntersected;
			}

			Vector2<int> intersection = new Vector2<int>(
				(int) Math.Ceiling((double) (other.B * C - B * other.C) / det),
				(int) Math.Ceiling((double) (A * other.C - other.A * C) / det));

			return intersection;
		}
	}
}
