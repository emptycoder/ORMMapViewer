using System;
using ORMMap.VectorTile.Geometry;

namespace ORMMapViewer.Model.Entitites
{
	public class Line
	{
		public static readonly Vector2<int> NotIntersected = new Vector2<int>(int.MinValue, int.MinValue);
		public readonly int A, B, C;
		public readonly Node node1;
		public readonly Node node2;

		public Line(Node first, Node second)
		{
			node1 = first;
			node2 = second;

			A = second.pos.Y - first.pos.Y;
			B = first.pos.X - second.pos.X;
			C = A * first.pos.X + B * first.pos.Y;
		}

		public bool IsVectorOnLine(Vector2<int> vector)
		{
			if (node1.pos.X == node2.pos.X)
			{
				return vector.X == node2.pos.X;
			}
			if (node1.pos.Y == node2.pos.Y)
			{
				return vector.Y == node2.pos.Y;
			}
			return (node1.pos.X - node2.pos.X) * (node1.pos.Y - node2.pos.Y) == (node2.pos.X - vector.X) * (node2.pos.Y - vector.Y);
		}

		private static bool IsVectorInside(Vector2<int> vector, Line l1, Line l2)
		{
			return l1.IsVectorOnLine(vector) && l2.IsVectorOnLine(vector);
		}

		public Vector2<int> CheckIntersection(Line other)
		{
			int det = A * other.B - other.A * B;

			if (det == 0)
			{
				return NotIntersected;
			}

			Vector2<int> intersection = new Vector2<int>(
				(int) Math.Ceiling((double) (other.B * C - B * other.C) / det),
				(int) Math.Ceiling((double) (A * other.C - other.A * C) / det));

			return IsVectorInside(intersection, this, other) ? intersection : NotIntersected;
		}
	}
}
