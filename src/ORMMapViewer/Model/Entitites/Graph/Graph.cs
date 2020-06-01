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
				nodes.Add(newNode);
			}
			else
			{
				nodes[index].TakeRelatives(newNode);
				nodes[index].UpdateRelatives();
			}
		}
	}
}
