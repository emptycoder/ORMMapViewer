using System;
using System.Collections.Generic;

namespace ORMMapViewer.Model.Entitites
{
	public class Graph
	{
		public List<Node> nodes = new List<Node>();

		public void AddNodesToList(List<Node> newNodes)
		{
			foreach (Node node in newNodes)
			{
				AddNode(node);
			}
		}

		public void AddNode(Node newNode)
		{
			int index = nodes.IndexOf(newNode);
			if (index == -1)
			{
				newNode.id = nodes.Count;
				nodes.Add(newNode);
			}
			else
			{
				newNode.id = -2;
				nodes[index].TakeNeighbours(newNode);
				newNode.SetRemoved();
				foreach (Node node in nodes)
				{
					node.neighbours.Remove(newNode);
				}
				// nodes[index].UpdateRelatives();
			}
		}
	}
}
