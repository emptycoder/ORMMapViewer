using System;
using System.Collections.Generic;
using ORMMap.VectorTile.Geometry;

namespace ORMMapViewer.Model.Entitites
{
	public class Node
	{
		private const int maxDistance = 1;
		public readonly Dictionary<Node, Weight> neighbours = new Dictionary<Node, Weight>();
		public int id = -1;
		public Vector2<int> pos;

		public Node(int x, int y)
		{
			pos = new Vector2<int>(x, y);
		}

		public Node AddNeighbour(Node node)
		{
			if (!neighbours.ContainsKey(node))
			{
				neighbours.Add(node, new LengthWeight(this, node));
			}

			return this;
		}

		public override bool Equals(object obj)
		{
			return obj is Node node &&
			       Math.Abs(pos.X - node.pos.X) < maxDistance &&
			       Math.Abs(pos.Y - node.pos.Y) < maxDistance;
		}

		public override int GetHashCode()
		{
			int hashCode = 1502939027;
			hashCode = hashCode * -1521134295 + pos.X.GetHashCode();
			hashCode = hashCode * -1521134295 + pos.Y.GetHashCode();
			return hashCode;
		}

		public void TakeNeighbours(Node other)
		{
			foreach (Node node in other.neighbours.Keys)
			{
				if (!neighbours.ContainsKey(node))
				{
					node.AddNeighbour(this);
					AddNeighbour(node);
				}
			}
		}

		public void UpdateNeighbours()
		{
			foreach (Node node in neighbours.Keys)
			{
				if (!node.neighbours.ContainsKey(this))
				{
					node.AddNeighbour(this);
				}
			}
		}

		public void SetRemoved()
		{
			foreach (Node node in neighbours.Keys)
			{
				node.neighbours.Remove(this);
			}
			neighbours.Clear();
		}
	}
}
