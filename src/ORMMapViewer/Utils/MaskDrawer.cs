using ORMMap.VectorTile.Geometry;
using ORMMapViewer.Model.Entitites;
using System;
using System.Drawing;

namespace ORMMapViewer.Utils
{
	public class MaskDrawer
	{
		const int startTileSize = 4096;
		const int t = 2;
		const int tile = startTileSize / t;

		private static int rectangleSize = (int) Math.Floor(Math.Sqrt(2.0 / t) * 10);
		private Line[,] mask = new Line[tile, tile];
		private Graph graph = new Graph();

		public void DrawMaskLine(Vector2<int> startPoint, Vector2<int> endPoint)
		{
			if ((startPoint.X > startTileSize || startPoint.Y > startTileSize) && (endPoint.X > startTileSize || endPoint.Y > startTileSize))
			{
				return;
			}
			// Convert to normal
			startPoint.X = startPoint.X / t;
			startPoint.Y = startPoint.Y / t;

			endPoint.X = endPoint.X / t;
			endPoint.Y = endPoint.Y / t;

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

			while (x != endPoint.X && y != endPoint.Y && x < tile && y < tile && x > 0 && y > 0)
			{
				if (mask[x, y] == null)
				{
					DrawRectangle(x, y, line);
				}
				else // Insert node
				{
					Line foundLine = mask[x, y];
					if (foundLine != line)
					{
						Graph.UnlinkNodes(foundLine.startNode, foundLine.endNode);
						Graph.UnlinkNodes(line.startNode, line.endNode);

						Node node = new Node(x, y);
						node = graph.AddNode(node);
						Graph.LinkNodes(node, foundLine.startNode);
						Graph.LinkNodes(node, foundLine.endNode);

						Graph.LinkNodes(node, line.startNode);
						Graph.LinkNodes(node, line.endNode);

						line.endNode = node;
						line = new Line(node, endNode);

						Line newLine = new Line(node, foundLine.endNode);
						foundLine.endNode = node;
						ReDrawLine(newLine);
						for (; ; )
						{
							if (!(x != endPoint.X && y != endPoint.Y && x < tile && y < tile && x > 0 && y > 0))
							{ return; }
							if (mask[x, y] == null)
							{
								break;
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

		private void DrawRectangle(int startX, int startY, Line filler)
		{
			for (int y = -rectangleSize; y <= rectangleSize; y++)
			{
				int startYY = y + startY;
				if (startYY < tile && startYY >= 0)
				{
					for (int x = -rectangleSize; x <= rectangleSize; x++)
					{
						int startXX = x + startX;
						if (startXX < tile && startXX >= 0)
						{
							mask[startXX, startYY] = filler;
						}
					}
				}
			}
		}

		private void ReDrawLine(Line line)
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

			while (x != line.endNode.pos.X && y != line.endNode.pos.Y && x < tile && y < tile && x > 0 && y > 0)
			{
				DrawRectangle(x, y, line);
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

		public void SaveToFile(string path)
		{
			Bitmap bitmap = new Bitmap(tile, tile);

			for (int x = 0; x < tile; x++)
			{
				for (int y = 0; y < tile; y++)
				{
					if (mask[x, y] != null)
					{
						bitmap.SetPixel(x, y, Color.Black);
					}
				}
			}

			bitmap.Save(path);
			bitmap.Dispose();
		}

		public Graph GetGraph()
		{
			foreach (Node node in graph.nodes)
			{
				node.pos.X *= t;
				node.pos.Y *= t;
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
