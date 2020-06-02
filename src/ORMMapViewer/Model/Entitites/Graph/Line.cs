using System;
using ORMMap.VectorTile.Geometry;

namespace ORMMapViewer.Model.Entitites
{
	public class Line
	{
		public static readonly Vector2<int> NotIntersected = new Vector2<int>(int.MinValue, int.MinValue);
		public readonly int A, B, C;
		public Node startNode;
		public Node endNode;

		public Line(Node first, Node second)
		{
			startNode = first;
			endNode = second;

			A = second.pos.Y - first.pos.Y;
			B = first.pos.X - second.pos.X;
			C = A * first.pos.X + B * first.pos.Y;
		}

		public bool IsVectorOnLine(Vector2<int> vector)
		{
			if (startNode.pos.X == endNode.pos.X)
			{
				return vector.X == endNode.pos.X;
			}
			if (startNode.pos.Y == endNode.pos.Y)
			{
				return vector.Y == endNode.pos.Y;
			}
			return (startNode.pos.X - endNode.pos.X) * (startNode.pos.Y - endNode.pos.Y) == (endNode.pos.X - vector.X) * (endNode.pos.Y - vector.Y);
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
