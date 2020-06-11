using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ORMMap.Model.Entitites;
using ORMMap.VectorTile;
using ORMMap.VectorTile.Geometry;
using ORMMapViewer.Model.Entitites;

namespace ORMMap
{
	public static class MVTDrawer
	{
		private static readonly Font font = new Font("Consolas", 6, FontStyle.Regular);

		private delegate void DrawDelegate(VectorTileFeature feature, Pallete pallete, Graphics graphics, double divisionRatio);

		private static readonly Dictionary<GeomType, DrawDelegate> featureDrawDictionary =
			new Dictionary<GeomType, DrawDelegate>
			{
				{GeomType.POLYGON, DrawPolygon},
				{GeomType.LINESTRING, DrawLineString},
				{GeomType.POINT, DrawPoint}
			};

		public static void DrawLayer(VectorTileLayer layer, Pallete pallete, Graphics graphics, double divisionRatio)
		{
			int featureCount = layer.FeatureCount();
			for (int i = 0; i < featureCount; i++)
			{
				VectorTileFeature feature = layer.GetFeature(i);
				if (feature.GeometryType == GeomType.UNKNOWN)
				{
					Console.WriteLine("Unknown feature: " + feature);
					continue;
				}

				featureDrawDictionary[feature.GeometryType](feature, pallete, graphics, divisionRatio);
			}
		}

		private static void DrawPolygon(VectorTileFeature feature, Pallete pallete, Graphics graphics, double divisionRatio)
		{
			List<List<Vector2<int>>> list = feature.Geometry<int>();
			foreach (List<Vector2<int>> item in list)
			{
				Point[] points = item.Select(vector2 => new Point((int)Math.Ceiling(vector2.X / divisionRatio), (int)Math.Ceiling(vector2.Y / divisionRatio))).ToArray();
				using (SolidBrush solidBrush = new SolidBrush(pallete.MainFillColor))
				{
					graphics.FillPolygon(solidBrush, points);
				}
				using (Pen pen = new Pen(pallete.MainDrawPen.Color, 2))
				{
					graphics.DrawPolygon(pen, points);
				}
			}
		}

		// TOOD: Add implimentation
		private static void DrawPoint(VectorTileFeature feature, Pallete pallete, Graphics graphics, double divisionRatio) { }

		private static void DrawLineString(VectorTileFeature feature, Pallete pallete, Graphics graphics, double divisionRatio)
		{
			Dictionary<string, object> props = feature.GetProperties();
			foreach (List<Vector2<int>> geometry in feature.Geometry<int>())
			{
				Point[] points = geometry.Select(vector2 => new Point((int)Math.Ceiling(vector2.X / divisionRatio), (int)Math.Ceiling(vector2.Y / divisionRatio))).ToArray();
				using (Pen pen = new Pen(pallete.MainDrawPen.Color, 2))
				{
					graphics.DrawLines(pen, points);
				}

				// Draw name of street
				//if (props.ContainsKey("name"))
				//{
				//	var text = (string)props["name"];
				//	foreach (Vector2<int> point in geometry)
				//	{
				//		graphics.DrawString(text, font, pallete.GetPropFillBrush("name"), point.X, point.Y);
				//	}
				//}
			}
		}

		public static void DrawLinks(IEnumerable<Node> roads, Graphics graphics, double divisionRatio)
		{
			using (Pen pen = new Pen(Color.Brown, 2))
			{
				foreach (Node node in roads)
				{
					foreach (KeyValuePair<Node, IWeight> nodeNeighbour in node.neighbours)
					{
						if (nodeNeighbour.Key.neighbours.ContainsKey(node))
						{
							graphics.DrawLine(pen,
								(int)Math.Ceiling(node.pos.X / divisionRatio),
								(int)Math.Ceiling(node.pos.Y / divisionRatio),
								(int)Math.Ceiling(nodeNeighbour.Key.pos.X / divisionRatio),
								(int)Math.Ceiling(nodeNeighbour.Key.pos.Y / divisionRatio)
							);
							graphics.FillEllipse(Brushes.Black, (int)Math.Ceiling(node.pos.X / divisionRatio), (int)Math.Ceiling(node.pos.Y / divisionRatio), 2, 2);
							graphics.FillEllipse(Brushes.Black, (int)Math.Ceiling(nodeNeighbour.Key.pos.X / divisionRatio), (int)Math.Ceiling(nodeNeighbour.Key.pos.Y / divisionRatio), 2, 2);
						}
					}
				}
			}
		}

		public static void DrawNodeIndices(IEnumerable<Node> roads, Graphics graphics, double divisionRatio)
		{
			int index = 0;
			using (SolidBrush brush = new SolidBrush(Color.Chartreuse))
			{
				foreach (Node node in roads)
				{
					graphics.DrawString(index++.ToString(), font, brush, (int)Math.Ceiling(node.pos.X / divisionRatio), (int)Math.Ceiling(node.pos.Y / divisionRatio));
					graphics.FillEllipse(Brushes.Red, (int)Math.Ceiling(node.pos.X / divisionRatio), (int)Math.Ceiling(node.pos.Y / divisionRatio), 2, 2);
				}
			}
		}

		public static void DrawGraphRoads(IEnumerable<Node> roads, Graphics graphics, double divisionRatio)
		{
			Point[] points = roads.Select(node => new Point((int)Math.Ceiling(node.pos.X / divisionRatio), (int)Math.Ceiling(node.pos.Y / divisionRatio))).ToArray();
			using (Pen pen = new Pen(Color.Red, 1))
			{
				graphics.DrawLines(pen, points);
			}
		}

		public static void DrawNode(Node node, Graphics graphics, double divisionRatio)
		{
			graphics.FillEllipse(Brushes.Red, (int)Math.Ceiling(node.pos.X / divisionRatio), (int)Math.Ceiling(node.pos.Y / divisionRatio), 5, 5);
		}

		public static void DrawText(string text, Vector2<int> point, Graphics graphics, double divisionRatio)
		{
			graphics.DrawString(text, font, Brushes.Red, (int)Math.Ceiling(point.X / divisionRatio), (int)Math.Ceiling(point.X / divisionRatio));
		}
	}
}
