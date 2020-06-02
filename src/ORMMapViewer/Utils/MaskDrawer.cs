using ORMMap.VectorTile.Geometry;
using ORMMapViewer.Model.Entitites;
using System;
using System.Drawing;

namespace ORMMapViewer.Utils
{
	public static class MaskDrawer
	{
		private static Line[,] mask = new Line[2048, 2048];
		private static Graph graph = new Graph();

		// 4096 / 50 = 81.92
		public static void DrawMaskLine(Vector2<int> startPoint, Vector2<int> endPoint)
		{
			// Convert to normal
			startPoint.X = startPoint.X / 2;
			startPoint.Y = startPoint.Y / 2;

			endPoint.X = endPoint.X / 2;
			endPoint.Y = endPoint.Y / 2;

			Node startNode = new Node(startPoint.X, startPoint.Y);
			Node endNode = new Node(endPoint.X, endPoint.Y);
			startNode = graph.AddNode(startNode);
			endNode = graph.AddNode(endNode);
			Graph.LinkNodes(startNode, endNode);

			Line line = new Line(startNode, endNode);

			Point dir = new Point(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
			float a = (float)Math.Sqrt(dir.X * dir.X + dir.Y * dir.Y);

			float dx = dir.X / a;
			float dy = dir.Y / a;

			int x = startPoint.X;
			int y = startPoint.Y;

			int stepX = (dx >= 0) ? 1 : -1;
			int stepY = (dy >= 0) ? 1 : -1;

			float tMaxX = (dx != 0) ? IntBound(startPoint.X, dx) : float.MaxValue;
			float tMaxY = (dy != 0) ? IntBound(startPoint.Y, dy) : float.MaxValue;

			float tDeltaX = (dx != 0) ? stepX / dx : float.MaxValue;
			float tDeltaY = (dy != 0) ? stepY / dy : float.MaxValue;

			while (x != endPoint.X && y != endPoint.Y && x < 2048 && y < 2048)
			{
				if (mask[x, y] == null)
				{
					mask[x, y] = line;
				}
				else // Insert
				{
					Line foundLine = mask[x, y];
					Graph.UnlinkNodes(foundLine.startNode, foundLine.endNode);

					Node node = new Node(x, y);
					node = graph.AddNode(node);
					Graph.LinkNodes(node, foundLine.startNode);
					Graph.LinkNodes(node, foundLine.endNode);

					line.endNode = node;
					line = new Line(node, endNode);

					Line newLine = new Line(node, foundLine.endNode);
					foundLine.endNode = node;
					ReDrawLine(newLine);
				}

				if (tMaxX < tMaxY)
				{
					x += stepX;
					tMaxX += tDeltaX;
				}
				else
				{
					y += stepY;
					tMaxY += tDeltaY;
				}
			}
		}

		private static void ReDrawLine(Line line)
		{
			Point dir = new Point(line.endNode.pos.X - line.startNode.pos.X, line.endNode.pos.Y - line.startNode.pos.Y);
			float a = (float)Math.Sqrt(dir.X * dir.X + dir.Y * dir.Y);

			float dx = dir.X / a;
			float dy = dir.Y / a;

			int x = line.startNode.pos.X;
			int y = line.startNode.pos.Y;

			int stepX = (dx >= 0) ? 1 : -1;
			int stepY = (dy >= 0) ? 1 : -1;

			float tMaxX = (dx != 0) ? IntBound(line.startNode.pos.X, dx) : float.MaxValue;
			float tMaxY = (dy != 0) ? IntBound(line.startNode.pos.Y, dy) : float.MaxValue;

			float tDeltaX = (dx != 0) ? stepX / dx : float.MaxValue;
			float tDeltaY = (dy != 0) ? stepY / dy : float.MaxValue;

			while (x != line.endNode.pos.X && y != line.endNode.pos.Y && x < 2048 && y < 2048)
			{
				mask[x, y] = line;
				if (tMaxX < tMaxY)
				{
					x += stepX;
					tMaxX += tDeltaX;
				}
				else
				{
					y += stepY;
					tMaxY += tDeltaY;
				}
			}
		}

		public static Graph GetGraph()
		{
			foreach (Node node in graph.nodes)
			{
				node.pos.X *= 2;
				node.pos.Y *= 2;
			}

			return graph;
		}

		private static float IntBound(float s, float ds)
		{
			if (Math.Round(s) == s && ds < 0)
			{
				return 0;
			}
			if (ds < 0)
			{
				return IntBound(-s, -ds);
			}
			else
			{
				s = (s % 1 + 1) % 1;
				return (1 - s) / ds;
			}
		}
	}
}
