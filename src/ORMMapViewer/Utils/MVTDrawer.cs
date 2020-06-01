using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ORMMap.Model.Entitites;
using ORMMap.VectorTile;
using ORMMap.VectorTile.Geometry;

namespace ORMMap
{
	public static class MVTDrawer
	{
		private static readonly Dictionary<GeomType, DrawDelegate> featureDrawDictionary =
			new Dictionary<GeomType, DrawDelegate>
			{
				{GeomType.POLYGON, DrawPolygon},
				{GeomType.LINESTRING, DrawLineString},
				{GeomType.POINT, DrawPoint}
			};

		public static void DrawLayer(VectorTileLayer layer, Pallete pallete, Graphics graphics)
		{
			var featureCount = layer.FeatureCount();
			for (var i = 0; i < featureCount; i++)
			{
				var feature = layer.GetFeature(i);
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
			using (var solidBrush = new SolidBrush(pallete.MainFillColor))
			{
				using (var pen = new Pen(pallete.MainDrawColor))
				{
					var points = new List<PointF>();
					var list = feature.Geometry<int>();
					foreach (var item in list)
					{
						points.Clear();

						foreach (var point in item) points.Add(new PointF(point.X, point.Y));

						graphics.FillPolygon(solidBrush, points.ToArray());
						graphics.DrawPolygon(pen, points.ToArray());
					}
				}
			}
		}

		private static void DrawPoint(VectorTileFeature feature, Pallete pallete, Graphics graphics)
		{
		}

		private static void DrawLineString(VectorTileFeature feature, Pallete pallete, Graphics graphics)
		{
			var props = feature.GetProperties();
			var geometry = feature.Geometry<int>()[0];
			var points = geometry.Select(vector2 => new Point(vector2.X, vector2.Y)).ToArray();

			Console.WriteLine(string.Join(",\n", points) + ",\n");

			// Draw name of street
			if (props.ContainsKey("name"))
			{
				using (var brush = new SolidBrush(pallete.GetPropFillColor("name")))
				{
					var text = (string) props["name"];
					foreach (var point in geometry)
						graphics.DrawString(text, SystemFonts.DefaultFont, brush, new Point(point.X, point.Y));
				}
			}

			using (var pen = new Pen(pallete.MainDrawColor, pallete.Thickness)) graphics.DrawLines(pen, points);
		}

		private delegate void DrawDelegate(VectorTileFeature feature, Pallete pallete, Graphics graphics);
	}
}
