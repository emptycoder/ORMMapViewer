using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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

		public static AStarPathSearchResult FindPath(Node start, Node end)
		{
			LinkedList<Node> path = new LinkedList<Node>();
			if (start.Equals(end))
			{
				path.AddFirst(start);
				return new AStarPathSearchResult(path, 0);
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

					return new AStarPathSearchResult(path, gScore.Sum((item) => item.Value));
				}

				foreach (KeyValuePair<Node, IWeight> pair in current.neighbours)
				{
					if (!pair.Key.Equals(current))
					{
						float tentativeGScore = gScore.ContainsKey(current) ? gScore[current] + pair.Value.Length : float.MaxValue;
						if (tentativeGScore < (gScore.ContainsKey(pair.Key) ? gScore[pair.Key] : float.MaxValue))
						{
							cameFrom[pair.Key] = current;
							if (!gScore.ContainsKey(pair.Key)) {
								gScore.Add(pair.Key, tentativeGScore);
							}
							else
							{
								gScore[pair.Key] = tentativeGScore;
							}
							if (!openSet.Contains(pair.Key))
							{
								openSet.Enqueue(pair.Key, tentativeGScore + DefaultHeuristic(pair.Key, end));
							}
						}
					}
				}
			}

			Console.WriteLine("No path found!");

			return null;
		}

		public sealed class AStarPathSearchResult
		{
			public LinkedList<Node> Path;
			public double Score;

			public AStarPathSearchResult(LinkedList<Node> path, double score)
			{
				this.Path = path;
				this.Score = score;
			}
		}
	}
}
