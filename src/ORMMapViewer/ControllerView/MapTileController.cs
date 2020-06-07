using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using ORMMap;
using ORMMap.Model.Entitites;
using ORMMap.VectorTile;
using ORMMap.VectorTile.Geometry;
using ORMMapViewer.Model.Entitites;
using ORMMapViewer.Utils;

namespace ORMMapViewer
{
	public partial class MainWindow
	{
		private static readonly Int32Collection defaultIndices = new Int32Collection
		{
			0, 1, 3, 1, 2, 3
		};

		private static readonly PointCollection defaultTextureCoordinates = new PointCollection
		{
			new System.Windows.Point(0, 0),
			new System.Windows.Point(-4096, 0),
			new System.Windows.Point(-4096, 4096),
			new System.Windows.Point(0, 4096)
		};

		private static AmbientLight defaultLight = new AmbientLight(System.Windows.Media.Color.FromRgb(255, 255, 255));
		public static Vector2<int> startPos = MercatorProjection.LatLngToTile(Settings.startPosition, Settings.zoom);
		private Dictionary<string, Model3DGroup> model3DCache = new Dictionary<string, Model3DGroup>(Settings.memoryMapModelsCacheOpacity);

		public double fovHInRadians;
		public double fovVInRadians;

		private void InitializeProjection()
		{
			fovHInRadians = camera.FieldOfView * Math.PI / 180;
			double aspect = this.Width / this.Height;
			fovVInRadians = ProjectionUtils.FovVFromAspectRatio(fovHInRadians, aspect);
		}

		private Projection CalculateProjection()
		{
			return ProjectionUtils.GetRectangleProjection(fovHInRadians, fovVInRadians, (int)camera.Position.Z);
		}

		public void UpdateScene(int tileSize)
		{
			tiles.Children.Clear();
			tiles.Children.Add(defaultLight);
			Projection projection = CalculateProjection();

			int tilesPerWidth = (int)Math.Ceiling(projection.Bottom / tileSize) + 1;
			int tilesPerHeight = (int)Math.Ceiling(projection.Left / tileSize) + 1;

			int startPosX = (int)Math.Floor((camera.Position.X - (projection.Bottom / 2)) / tileSize);
			int startPosY = (int)Math.Floor((camera.Position.Y - (projection.Left / 2)) / tileSize);

			double cZoom = dataController.ConvertToMapZoom(Settings.zoom);
			Vector2<int> startTileCoordinations = MercatorProjection.LatLngToTile(Settings.startPosition, cZoom);
			Vector3<double> lonLatZoom = new Vector3<double>(
				startTileCoordinations.X,
				startTileCoordinations.Y,
				cZoom
			);

			for (int x = startPosX; x < startPosX + tilesPerWidth; x++)
			{
				for (int y = startPosY; y < startPosY + tilesPerHeight; y++)
				{
					lonLatZoom.X = startTileCoordinations.X - x;
					lonLatZoom.Y = startTileCoordinations.Y + y;
					string key = x + " " + y;
					if (!model3DCache.TryGetValue(key, out Model3DGroup model3DGroup))
					{
						model3DGroup = CacheModel3DGroup(key, CreateTile(x, y, lonLatZoom));
					}

					if (zoom > 18.5)
					{
						tiles.Children.Add(model3DGroup);
					}
					else
					{
						tiles.Children.Add(model3DGroup.Children[0]);
					}
				}
			}
		}

		private GeometryModel3D CreateTileModel(int x, int y, BitmapSource drawingObjects)
		{
			GeometryModel3D model = new GeometryModel3D
			{
				Geometry = new MeshGeometry3D
				{
					Positions = new Point3DCollection
					{
						new Point3D(x * 4096, y * 4096, 0),
						new Point3D((x + 1) * 4096, y * 4096, 0),
						new Point3D((x + 1) * 4096, (y + 1) * 4096, 0),
						new Point3D(x * 4096, (y + 1) * 4096, 0)
					},
					TriangleIndices = defaultIndices,
					TextureCoordinates = defaultTextureCoordinates
				},
				Material = new DiffuseMaterial(new ImageBrush
				{
					//ViewportUnits = BrushMappingMode.Absolute,
					TileMode = TileMode.Tile,
					//Viewport = new Rect(new Point(0, 0), new Point(size, size)),
					//ViewboxUnits = BrushMappingMode.Absolute,
					Stretch = Stretch.Fill,
					AlignmentX = AlignmentX.Left,
					AlignmentY = AlignmentY.Top,
					ImageSource = drawingObjects
				})
			};

			return model;
		}

		private Model3DGroup CreateTile(int x, int y, Vector3<double> lonLatZoom)
		{
			VectorTileObj vectorTileObj = dataController.GetData(lonLatZoom);

			ReadOnlyCollection<string> layers = vectorTileObj.LayerNames();
			Bitmap drawingObjects = new Bitmap(512, 512);
			using (Graphics graphics = Graphics.FromImage(drawingObjects))
			{
				foreach (string layerName in drawingLayersPallete.Keys)
				{
					if (layers.Contains(layerName))
					{
						VectorTileLayer layer = vectorTileObj.GetLayer(layerName);
						MVTDrawer.DrawLayer(layer, drawingLayersPallete[layerName], graphics, 4096 / 256);
					}
				}

				// Graph graph = dataController.GetRoads(lonLatZoom);
				// LinkedList<Node> list = AStarPathSearch.FindPath(graph.nodes[423], graph.nodes[83]);
				// if (list != null && list.Count > 1)
				// {
				// 	MVTDrawer.DrawGraphRoads(list, graphics);
				// }
				// MVTDrawer.DrawNodeIndices(graph.nodes, graphics);
			}

			Model3DGroup model3DGroup = new Model3DGroup();
			model3DGroup.Children.Add(CreateTileModel(x, y, ImageUtils.GetImageStream(drawingObjects)));
			foreach (string layerName in modelsLayersPallete.Keys)
			{
				if (layers.Contains(layerName))
				{
					MVTModelCreator.CreateLayer(vectorTileObj.GetLayer(layerName), modelsLayersPallete[layerName], model3DGroup);
				}
			}

			return model3DGroup;
		}

		private Model3DGroup CacheModel3DGroup(string key, Model3DGroup model3DGroup)
		{
			if (model3DCache.Count == Settings.memoryMapModelsCacheOpacity)
			{
				model3DCache.Remove(model3DCache.Keys.First());
			}

			model3DCache.Add(key, model3DGroup);

			return model3DGroup;
		}
	}
}
