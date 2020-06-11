using System.Collections.Generic;
using Newtonsoft.Json;
using ORMMap.VectorTile.Geometry;

namespace ORMMapViewer.Model.Entitites
{
	public class Node
	{
		public Dictionary<Node, IWeight> neighbours = new Dictionary<Node, IWeight>();
		public int id = -1;
		public Vector2<int> pos;

		[JsonConstructor]
		public Node() {}

		public Node(int x, int y)
		{
			pos = new Vector2<int>(x, y);
		}

		public Node(Vector2<int> pos)
		{
			this.pos = pos;
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
