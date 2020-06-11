using ORMMap;
using ORMMap.VectorTile.Geometry;
using ORMMapViewer.Model.Entitites;
using System;
using System.Drawing;

namespace ORMMapViewer.Utils
{
	public class MaskGraphCreator
	{
		private Segment[] borders = new Segment[4];
		private readonly int startTileSize;
		private readonly int tileSize;
		private readonly int tileSizeSubstract1;

		private static int rectangleSize = (int) Math.Floor(Math.Sqrt(2.0 / Settings.accuracy) * 10);
		private Segment[,] mask;
		private Graph graph = new Graph();

		public MaskGraphCreator(int tileSize)
		{
			this.startTileSize = tileSize;
			this.tileSize = startTileSize / Settings.accuracy;
			this.tileSizeSubstract1 = tileSize - 1;
			this.mask = new Segment[tileSize, tileSize];

			borders[0] = new Segment(new Vector2<int>(0, 0), new Vector2<int>(0, 1));
			borders[1] = new Segment(new Vector2<int>(0, 0), new Vector2<int>(1, 0));

			borders[2] = new Segment(new Vector2<int>(tileSize, tileSize), new Vector2<int>(tileSize - 1, tileSize));
			borders[3] = new Segment(new Vector2<int>(tileSize, tileSize), new Vector2<int>(tileSize, tileSize - 1));
		}

		// TODO: Create border nodes
		private Vector2<int> Normal(Vector2<int> startPoint, double angle)
		{
			if (startPoint.Y >= tileSize)
			{
				startPoint.X -= (int)Math.Floor(startPoint.GetDistance(startPoint.X, tileSizeSubstract1) * Math.Tan(angle));
				startPoint.Y = tileSizeSubstract1;
			}

			if (startPoint.X >= tileSize)
			{
				startPoint.Y -= (int)Math.Floor(startPoint.GetDistance(startPoint.Y, tileSizeSubstract1) * Math.Tan(angle));
				startPoint.X = tileSizeSubstract1;
			}

			if (startPoint.Y < 0)
			{
				startPoint.X += (int)Math.Floor(startPoint.GetDistance(startPoint.X, tileSizeSubstract1) * Math.Tan(Math.PI - angle));
				startPoint.Y = 1;
			}

			if (startPoint.X < 0)
			{
				startPoint.Y += (int)Math.Floor(startPoint.GetDistance(startPoint.Y, tileSizeSubstract1) * Math.Tan(Math.PI - angle));
				startPoint.X = 1;
			}

			return startPoint;
		}

		public void DrawMaskLine(Vector2<int> startPoint, Vector2<int> endPoint)
		{
			// For fast testes (1/4 of map tile)
			//if ((startPoint.X > startTileSize || startPoint.Y > startTileSize) && (endPoint.X > startTileSize || endPoint.Y > startTileSize))
			//{
			//	return;
			//}
			// Convert coords to (4096 / t) scale
			startPoint = startPoint.Divide(Settings.accuracy);
			endPoint = endPoint.Divide(Settings.accuracy);

			//double angle = Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X);
			//startPoint = Normal(startPoint, angle);
			//endPoint = Normal(endPoint, angle);

			Node startNode = new Node(startPoint.X, startPoint.Y);
			Node endNode = new Node(endPoint.X, endPoint.Y);
			startNode = graph.AddNode(startNode);
			endNode = graph.AddNode(endNode);
			graph.LinkNodes(startNode, endNode);

			Segment segment = new Segment(startNode, endNode);

			// Raycast from startPoint to endPoint
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

			while (x != endPoint.X && y != endPoint.Y && x < tileSize && y < tileSize && x > 0 && y > 0)
			{
				if (mask[x, y] == null)
				{
					DrawRectangle(x, y, segment);
				}
				else // Insert node
				{
					Segment foundSegment = mask[x, y];
					if (foundSegment != segment)
					{
						graph.UnlinkNodes(foundSegment.startNode, foundSegment.endNode);
						graph.UnlinkNodes(segment.startNode, segment.endNode);

						Node node = new Node(x, y);
						node = graph.AddNode(node);
						graph.LinkNodes(node, foundSegment.startNode);
						graph.LinkNodes(node, foundSegment.endNode);

						graph.LinkNodes(node, segment.startNode);
						graph.LinkNodes(node, segment.endNode);

						segment.endNode = node;
						segment = new Segment(node, endNode);

						Segment newLine = new Segment(node, foundSegment.endNode);
						foundSegment.endNode = node;
						ReDrawLine(newLine);
						for (; ; )
						{
							if (!(x != endPoint.X && y != endPoint.Y && x < tileSize && y < tileSize && x > 0 && y > 0))
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

							//if (x == tileSize || y == tileSize || x == 0 || y == 0)
							//{
							//	graph.UnlinkNodes(line.startNode, line.endNode);

							//	node = new Node(x - stepX, y - stepY);
							//	node = graph.AddNode(node);
							//	graph.LinkNodes(node, line.startNode);
							//	graph.LinkNodes(node, line.endNode);

							//	return;
							//}
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

				//if (x == tileSize || y == tileSize || x == 0 || y == 0)
				//{
				//	graph.UnlinkNodes(line.startNode, line.endNode);

				//	Node node = new Node(x - stepX, y - stepY);
				//	node = graph.AddNode(node);
				//	graph.LinkNodes(node, line.startNode);
				//	graph.LinkNodes(node, line.endNode);

				//	return;
				//}
			}
		}

		private void DrawRectangle(int startX, int startY, Segment filler)
		{
			for (int y = -rectangleSize; y <= rectangleSize; y++)
			{
				int startYY = y + startY;
				if (startYY < tileSize && startYY >= 0)
				{
					for (int x = -rectangleSize; x <= rectangleSize; x++)
					{
						int startXX = x + startX;
						if (startXX < tileSize && startXX >= 0)
						{
							mask[startXX, startYY] = filler;
						}
					}
				}
			}
		}

		private void ReDrawLine(Segment line)
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

			while (x != line.endNode.pos.X && y != line.endNode.pos.Y && x < tileSize && y < tileSize && x > 0 && y > 0)
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

				//if (x == tileSize || y == tileSize || x == 0 || y == 0)
				//{
				//	graph.UnlinkNodes(line.startNode, line.endNode);

				//	Node node = new Node(x - stepX, y - stepY);
				//	node = graph.AddNode(node);
				//	graph.LinkNodes(node, line.startNode);
				//	graph.LinkNodes(node, line.endNode);

				//	return;
				//}
			}
		}

		public void SaveToFile(string path)
		{
			using (Bitmap bitmap = new Bitmap(tileSize, tileSize))
			{
				for (int x = 0; x < tileSize; x++)
				{
					for (int y = 0; y < tileSize; y++)
					{
						if (mask[x, y] != null)
						{
							bitmap.SetPixel(x, y, Color.Black);
						}
					}
				}

				bitmap.Save(path);
			}
		}

		public Graph GetGraph()
		{
			foreach (Node node in graph.nodes)
			{
				node.pos = node.pos.Multiply(Settings.accuracy);
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
