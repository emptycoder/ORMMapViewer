using System.Collections.Generic;
using ORMMap.VectorTile.Geometry;
using ORMMapViewer.Utils;

namespace ORMMapViewer.Model.Entitites
{
	public class Graph
	{
		public List<Node> nodes = new List<Node>();

		public void LinkNodes(Node first, Node second)
		{
			first.AddNeighbour(second);
			second.AddNeighbour(first);
		}
		
		public void UnlinkNodes(Node first, Node second)
		{
			first.neighbours.Remove(second);
			second.neighbours.Remove(first);
		}

		public Node AddNode(Node newNode)
		{
			int index = -1;
			for (int i = 0; i < nodes.Count; i++)
			{
				if (nodes[i].pos.GetDistance(newNode.pos) < 20)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				return nodes[index];
			}

			newNode.id = nodes.Count;
			nodes.Add(newNode);

			return newNode;
		}

		public Node FindNearest(Vector2<double> pos)
		{
			Node nearestNode = null;
			double minDistance = double.MaxValue;

			foreach (Node node in nodes)
			{
				double distance = node.pos.GetDistance(pos);
				if (distance < minDistance)
				{
					nearestNode = node;
					minDistance = distance;
				}
			}

			return nearestNode;
		}
	}
}
