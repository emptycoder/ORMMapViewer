using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using ORMMapViewer.Model.Entitites;
using Priority_Queue;

namespace ORMMapViewer.Utils
{
	public class AStarPathSearch
	{
		private static float DefaultHeuristic(Node first, Node second)
		{
			return (float) Math.Sqrt(Math.Pow(first.pos.X - second.pos.X, 2) + Math.Pow(first.pos.Y - second.pos.Y, 2));
		}
		
		public static LinkedList<Node> FindPath(Node start, Node end)
		{
			LinkedList<Node> path = new LinkedList<Node>();
			if (start.Equals(end))
			{
				path.AddFirst(start);
				return path;
			}

			SimplePriorityQueue<Node> openSet = new SimplePriorityQueue<Node>();
			openSet.Enqueue(start, DefaultHeuristic(start, end));
			
			Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

			Dictionary<Node, float> gScore = new Dictionary<Node, float> {{start, 0}};

			while (openSet.Count > 0)
			{
				Node current = openSet.Dequeue();
				if (current.Equals(end))
				{
					path.AddFirst(current);
					while (!current.Equals(start))
					{
						current = cameFrom[current];
						path.AddFirst(current);
					}
					return path;
				}

				foreach (var pair in current.relatives)
				{
					if (!pair.Key.Equals(current))
					{
						float tentativeGScore = gScore.ContainsKey(current)?gScore[current]:float.MaxValue + pair.Value.Calculate();
						if (tentativeGScore < (gScore.ContainsKey(pair.Key)?gScore[pair.Key]:float.MaxValue))
						{
							cameFrom[pair.Key] = current;
							gScore[pair.Key] = tentativeGScore;
							if (!openSet.Contains(pair.Key))
							{
								openSet.Enqueue(pair.Key, tentativeGScore + DefaultHeuristic(pair.Key, end));
							}
						}
					}
				}
			}

			return null;
		}
	}
}
