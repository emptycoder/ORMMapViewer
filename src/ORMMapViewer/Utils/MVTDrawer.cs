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

		private static readonly Dictionary<GeomType, DrawDelegate> featureDrawDictionary =
			new Dictionary<GeomType, DrawDelegate>
			{
				{GeomType.POLYGON, DrawPolygon},
				{GeomType.LINESTRING, DrawLineString},
				{GeomType.POINT, DrawPoint}
			};

		public static void DrawLayer(VectorTileLayer layer, Pallete pallete, Graphics graphics)
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

				featureDrawDictionary[feature.GeometryType](feature, pallete, graphics);
			}
		}

		private static void DrawPolygon(VectorTileFeature feature, Pallete pallete, Graphics graphics)
		{
			List<PointF> points = new List<PointF>();
			List<List<Vector2<int>>> list = feature.Geometry<int>();
			foreach (List<Vector2<int>> item in list)
			{
				points.Clear();

				foreach (Vector2<int> point in item)
				{
					points.Add(new PointF(point.X, point.Y));
				}

				graphics.FillPolygon(pallete.GetMainFillBrush(), points.ToArray());
				graphics.DrawPolygon(pallete.GetMainDrawPen(), points.ToArray());
			}
		}

		private static void DrawPoint(VectorTileFeature feature, Pallete pallete, Graphics graphics)
		{
		}

		private static void DrawLineString(VectorTileFeature feature, Pallete pallete, Graphics graphics)
		{
			Dictionary<string, object> props = feature.GetProperties();
			List<Vector2<int>> geometry = feature.Geometry<int>()[0];
			Point[] points = geometry.Select(vector2 => new Point(vector2.X, vector2.Y)).ToArray();

			// Console.WriteLine(string.Join(",\n", points) + ",\n");
			graphics.DrawLines(pallete.GetMainDrawPen(), points);

			// Draw name of street
			if (props.ContainsKey("name"))
			{
				var text = (string)props["name"];
				foreach (Vector2<int> point in geometry)
				{
					graphics.DrawString(text, font, pallete.GetPropFillBrush("name"), new Point(point.X, point.Y));
				}
			}
		}

		public static void DrawNodeIndices(List<Node> roads, Graphics graphics)
		{
			int index = 0;
			// Console.WriteLine(string.Join(",\n", roads.Select((node, key) => key + ") " + node.pos)));

			using (SolidBrush brush = new SolidBrush(Color.Chartreuse))
			{
				foreach (Node node in roads)
				{
					graphics.DrawString(index++.ToString(), font, brush, new Point(node.pos.X, node.pos.Y));
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

		private delegate void DrawDelegate(VectorTileFeature feature, Pallete pallete, Graphics graphics);
	}
}
