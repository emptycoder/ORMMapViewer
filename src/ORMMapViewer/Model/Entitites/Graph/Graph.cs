using System.Collections.Generic;
using ORMMap.VectorTile.Geometry;

namespace ORMMapViewer.Model.Entitites
{
	public class Graph
	{
		public List<Node> nodes = new List<Node>();

		public static void LinkNodes(Node first, Node second)
		{
			first.AddNeighbour(second);
			second.AddNeighbour(first);
		}
		
		public static void UnlinkNodes(Node first, Node second)
		{
			first.neighbours.Remove(second);
			second.neighbours.Remove(first);
		}

		public Node AddNode(Node newNode)
		{
			int index = nodes.IndexOf(newNode);
			if (index != -1)
			{
				return nodes[index];
			}

			newNode.id = nodes.Count;
			nodes.Add(newNode);
			return newNode;
		}

		public void CheckAndAddIntersection(Line l1, Line l2)
		{
			Vector2<int> intersection = l1.CheckIntersection(l2);
			if (!intersection.Equals(Line.NotIntersected))
			{
				Node node = new Node(intersection.X, intersection.Y);
				node = AddNode(node);
				UnlinkNodes(l1.node1, l1.node2);
				UnlinkNodes(l2.node1, l2.node2);
				
				LinkNodes(node, l1.node1);
				LinkNodes(node, l1.node2);
				LinkNodes(node, l2.node1);
				LinkNodes(node, l2.node2);
			}
		}
	}
}
