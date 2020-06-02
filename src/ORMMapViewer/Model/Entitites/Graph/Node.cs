using System;
using System.Collections.Generic;
using ORMMap.VectorTile.Geometry;

namespace ORMMapViewer.Model.Entitites
{
	public class Node
	{
		private const int maxDistance = 10;
		public readonly Dictionary<Node, Weight> neighbours = new Dictionary<Node, Weight>();
		public int id = -1;
		public Vector2<int> pos;

		public Node(int x, int y)
		{
			pos = new Vector2<int>(x, y);
		}

		public void AddNeighbour(Node node)
		{
			if (!neighbours.ContainsKey(node))
			{
				neighbours.Add(node, new LengthWeight(this, node));
			}
		}

		public void RemoveNeighbour(Node node)
		{
			if (neighbours.ContainsKey(node))
			{
				neighbours.Remove(node);
			}
		}

		public override bool Equals(object obj)
		{
			return obj is Node node &&
			       Math.Abs(pos.X - node.pos.X) < maxDistance &&
			       Math.Abs(pos.Y - node.pos.Y) < maxDistance;
		}

		/*public override int GetHashCode()
		{
			int hashCode = 1502939027;
			hashCode = hashCode * -1521134295 + pos.X.GetHashCode();
			hashCode = hashCode * -1521134295 + pos.Y.GetHashCode();
			return hashCode;
		}*/

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

		public override string ToString()
		{
			return $"Id: {id}, pos: {pos}";
		}
	}
}
