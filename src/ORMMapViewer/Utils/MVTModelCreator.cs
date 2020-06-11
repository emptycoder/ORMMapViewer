using HelixToolkit.Wpf;
using ORMMap.Model.Entitites;
using ORMMap.VectorTile;
using ORMMap.VectorTile.Geometry;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ORMMapViewer.Utils
{
	public static class MVTModelCreator
	{
		private delegate void DrawDelegate(VectorTileFeature feature, Pallete pallete, Model3DGroup model3DGroup, Vector2<int> shiftCoords);

		private static readonly Dictionary<GeomType, DrawDelegate> featureCreateDictionary =
			new Dictionary<GeomType, DrawDelegate>
			{
				{GeomType.POLYGON, CreatePolygon},
				{GeomType.LINESTRING, CreateLineString},
				{GeomType.POINT, CreatePoint}
			};

		public static void CreateLayer(VectorTileLayer layer, Pallete pallete, Model3DGroup model3DGroup, Vector2<int> shiftCoords)
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

				featureCreateDictionary[feature.GeometryType](feature, pallete, model3DGroup, shiftCoords);
			}
		}

		private static void CreatePolygon(VectorTileFeature feature, Pallete pallete, Model3DGroup model3DGroup, Vector2<int> shiftCoords)
		{
			PointCollection points = new PointCollection();
			List<List<Vector2<int>>> list = feature.Geometry<int>();
			foreach (List<Vector2<int>> item in list)
			{
				points.Clear();

				foreach (Vector2<int> point in item)
				{
					points.Add(new Point(point.X + shiftCoords.X, point.Y + shiftCoords.Y));
				}

				points.RemoveAt(points.Count - 1);

				var model = new GeometryModel3D();
				var meshbuilder = new MeshBuilder(true, true);

				var result = CuttingEarsTriangulator.Triangulate(points);

				List<int> tri = new List<int>();
				for (int i = 0; i < result.Count; i++)
				{
					tri.Add(result[i]);
					if (tri.Count == 3)
					{
						//Console.WriteLine("Triangle " + (i / 3).ToString() + " : " + tri[0].ToString() + ", " + tri[1].ToString() + ", " + tri[2].ToString());
						meshbuilder.AddTriangle(new Point3D(points[tri[0]].X, points[tri[0]].Y, 1),
							new Point3D(points[tri[1]].X, points[tri[1]].Y, 1),
							new Point3D(points[tri[2]].X, points[tri[2]].Y, 1));
						tri.Clear();
					}
				}

				model.Geometry = meshbuilder.ToMesh();
				model.Material = MaterialHelper.CreateMaterial(pallete.MainFillColor.ToMediaColor());

				model3DGroup.Children.Add(model);
			}
		}

		// TOOD: Add implimentation
		private static void CreatePoint(VectorTileFeature feature, Pallete pallete, Model3DGroup model3DGroup, Vector2<int> shiftCoords) { }

		// TOOD: Add implimentation
		private static void CreateLineString(VectorTileFeature feature, Pallete pallete, Model3DGroup model3DGroup, Vector2<int> shiftCoords) { }
	}
}
