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
		private static readonly Font font = new Font("Consolas", 48, FontStyle.Regular);

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
				Point[] points = item.Select(vector2 => new Point((int)Math.Floor(vector2.X / divisionRatio), (int)Math.Floor(vector2.Y / divisionRatio))).ToArray();
				graphics.FillPolygon(pallete.GetMainFillBrush(), points);
				graphics.DrawPolygon(pallete.GetMainDrawPen(), points);
			}
		}

		// TOOD: Add implimentation
		private static void DrawPoint(VectorTileFeature feature, Pallete pallete, Graphics graphics, double divisionRatio) { }

		private static void DrawLineString(VectorTileFeature feature, Pallete pallete, Graphics graphics, double divisionRatio)
		{
			Dictionary<string, object> props = feature.GetProperties();
			foreach (List<Vector2<int>> geometry in feature.Geometry<int>())
			{
				Point[] points = geometry.Select(vector2 => new Point((int)Math.Floor(vector2.X / divisionRatio), (int)Math.Floor(vector2.Y / divisionRatio))).ToArray();
				graphics.DrawLines(pallete.GetMainDrawPen(), points);

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

		public static void DrawLinks(List<Node> roads, Graphics graphics)
		{
			using (Pen pen = new Pen(Color.Brown, 7))
			{
				foreach (Node node in roads)
				{
					foreach (var nodeN in node.neighbours)
					{
						if (nodeN.Key.neighbours.ContainsKey(node))
						{
							graphics.DrawLine(pen, node.pos.X, node.pos.Y, nodeN.Key.pos.X, nodeN.Key.pos.Y);
						}
					}
				}
			}
		}

		public static void DrawNodeIndices(List<Node> roads, Graphics graphics)
		{
			int index = 0;
			using (SolidBrush brush = new SolidBrush(Color.Chartreuse))
			{
				foreach (Node node in roads)
				{
					graphics.DrawString(index++.ToString(), font, brush, node.pos.X, node.pos.Y);
					graphics.FillEllipse(Brushes.Red, node.pos.X, node.pos.Y, 20, 20);
				}
			}
		}

		public static void DrawGraphRoads(LinkedList<Node> roads, Graphics graphics)
		{
			Point[] points = roads.Select(node => new Point(node.pos.X, node.pos.Y)).ToArray();
			using (Pen pen = new Pen(Color.Red, 7))
			{
				graphics.DrawLines(pen, points);
			}
		}
	}
}
