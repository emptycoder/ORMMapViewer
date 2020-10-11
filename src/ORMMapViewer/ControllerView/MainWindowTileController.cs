using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using ORMMap;
using ORMMap.Model.Entitites;
using ORMMap.VectorTile;
using ORMMap.VectorTile.Geometry;
using ORMMapViewer.Model.Entitites;
using ORMMapViewer.Utils;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

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
			new Point(0, 0),
			new Point(-4096, 0),
			new Point(-4096, 4096),
			new Point(0, 4096)
		};

		private static readonly AmbientLight defaultLight = new AmbientLight(Color.FromRgb(255, 255, 255));
		public static Vector2<double> startPos = MercatorProjection.LatLngToTile(Settings.startPosition, Settings.zoom);

		private static readonly ConcurrentExclusiveSchedulerPair schedulerPair = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, 1);
		private static readonly TaskFactory taskFactory = new TaskFactory(schedulerPair.ExclusiveScheduler);
		private readonly Dictionary<Vector2<int>, Model3D> currentTiles = new Dictionary<Vector2<int>, Model3D>();

		private readonly Dictionary<string, MapTile> mapTileCache = new Dictionary<string, MapTile>(Settings.memoryMapTilesCacheOpacity);

		public double fovHInRadians;
		public double fovVInRadians;
		
		private int oldEndX;
		private int oldEndY;
		private int oldStartX;
		private int oldStartY;
		private int oldZoom;

		private void InitializeProjection()
		{
			fovHInRadians = camera.FieldOfView * Math.PI / 180;
			double aspect = Width / Height;
			fovVInRadians = ProjectionUtils.FovVFromAspectRatio(fovHInRadians, aspect);
		}

		private Projection CalculateProjection()
		{
			return ProjectionUtils.GetRectangleProjection(fovHInRadians, fovVInRadians, (int) camera.Position.Z);
		}

		public void UpdateScene(int tileSize)
		{
			taskFactory.StartNew(() =>
			{
				// TODO: Trigger points for update scene instead recreate every control

				int tilesPerWidth = 0, tilesPerHeight = 0, startPosX = 0, startPosY = 0;

				Dispatcher.Invoke(() =>
				{
					Projection projection = CalculateProjection();
					tilesPerWidth = (int) Math.Ceiling(projection.Bottom / tileSize) + 7;
					tilesPerHeight = (int) Math.Ceiling(projection.Left / tileSize) + 4;

					// TileSize for tangram always 4096
					startPosX = (int) Math.Floor((camera.Position.X - projection.Bottom / 2) / tileSize)-3;
					startPosY = (int) Math.Floor((camera.Position.Y - projection.Left / 2) / tileSize)-2;
				});
				double cZoom = dataController.ConvertToMapZoom(zoom);

				List<Vector2<int>> tilesToAdd = new List<Vector2<int>>();
				List<Vector2<int>> tilesToRemove = new List<Vector2<int>>();

				for (int x = oldStartX; x < oldEndX; x++)
				{
					for (int y = oldStartY; y < oldEndY; y++)
					{
						tilesToRemove.Add(new Vector2<int>(x, y));
					}
				}

				for (int x = startPosX; x < startPosX + tilesPerWidth; x++)
				{
					for (int y = startPosY; y < startPosY + tilesPerHeight; y++)
					{
						if (oldZoom == (int) cZoom)
						{
							tilesToRemove.Remove(new Vector2<int>(x, y));
						}

						tilesToAdd.Add(new Vector2<int>(x, y));
					}
				}

				if (oldZoom == (int) cZoom)
				{
					for (int x = oldStartX; x < oldEndX; x++)
					{
						for (int y = oldStartY; y < oldEndY; y++)
						{
							tilesToAdd.Remove(new Vector2<int>(x, y));
						}
					}
				}

				oldStartX = startPosX;
				oldStartY = startPosY;
				oldEndX = startPosX + tilesPerWidth;
				oldEndY = startPosY + tilesPerHeight;
				oldZoom = (int) cZoom;
				
				Vector2<int> startTileCoordinations = MercatorProjection.LatLngToTile(Settings.startPosition, cZoom).ToVectorInt();

				List<Task> tasks = new List<Task>();

				foreach (Vector2<int> tile in tilesToRemove)
				{
					Dispatcher.Invoke(() =>
					{
						tiles.Children.Remove(currentTiles[tile]); 
						currentTiles.Remove(tile);
					});
				}

				foreach (Vector2<int> tile in tilesToAdd)
				{
					Vector3<double> lonLatZoom = new Vector3<double>(
						startTileCoordinations.X - tile.X,
						startTileCoordinations.Y + tile.Y,
						cZoom
					);
					string tileHash = lonLatZoom.X + " " + lonLatZoom.Y + " " + cZoom;
					if (mapTileCache.TryGetValue(tileHash, out MapTile mapTile))
					{
						Dispatcher.Invoke(() =>
						{
							tiles.Children.Add(mapTile.Model3DGroup.Children[0]);
							currentTiles.Add(tile, mapTile.Model3DGroup.Children[0]);
						});
					}
					else
					{
						tasks.Add(ProcessMapTile(tileHash, tile.X, tile.Y, lonLatZoom, TimeSpan.Zero));
					}
				}

				foreach (Task task in tasks)
				{
					task.Wait();
				}
			});
		}

		private Task ProcessMapTile(string tileHash, int x, int y, Vector3<double> lonLatZoom, TimeSpan delay)
		{
			return Task.Run(async delegate
			{
				await Task.Delay(delay);
				try
				{
					MapTile mapTile = CacheMapTile(tileHash, CreateTile(x, y, lonLatZoom));
					Dispatcher.Invoke(() =>
					{
						tiles.Children.Add(mapTile.Model3DGroup.Children[0]);
						currentTiles.Add(new Vector2<int>(x, y), mapTile.Model3DGroup.Children[0]);
					});
				}
				catch (WebException ex)
				{
					delay = delay == TimeSpan.Zero ? TimeSpan.FromSeconds(1) : TimeSpan.FromSeconds(delay.Seconds*2);
					Console.WriteLine($"Can't download tile ({x}, {y}) data: {ex.Message}, retrying again in {delay.Seconds} seconds");
					ProcessMapTile(tileHash, x, y, lonLatZoom, delay);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				} // TODO: NLog error handler
			});
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
					TileMode = TileMode.Tile,
					Stretch = Stretch.Fill,
					AlignmentX = AlignmentX.Left,
					AlignmentY = AlignmentY.Top,
					ImageSource = drawingObjects
				})
			};

			return model;
		}

		private MapTile CreateTile(int x, int y, Vector3<double> lonLatZoom)
		{
			VectorTileObj vectorTileObj = dataController.GetData(lonLatZoom);

			ReadOnlyCollection<string> layers = vectorTileObj.LayerNames();
			Bitmap drawingObjects = new Bitmap(256, 256);
			Graph graph = dataController.GetRoads(lonLatZoom);
			using (Graphics graphics = Graphics.FromImage(drawingObjects))
			{
				foreach (string layerName in drawingLayersPallete.Keys)
				{
					if (layers.Contains(layerName))
					{
						VectorTileLayer layer = vectorTileObj.GetLayer(layerName);
						MVTDrawer.DrawLayer(layer, drawingLayersPallete[layerName], graphics, 4096 / 128);
					}
				}
				MVTDrawer.DrawLinks(graph.nodes, graphics, 32);
			}

			return Dispatcher.Invoke(() =>
			{
				Model3DGroup model3DGroup = new Model3DGroup();
				model3DGroup.Children.Add(CreateTileModel(x, y, ImageUtils.GetImageStream(drawingObjects)));
				return new MapTile(model3DGroup);
			});
		}

		private MapTile CacheMapTile(string key, MapTile mapTile)
		{
			if (mapTileCache.Count == Settings.memoryMapTilesCacheOpacity)
			{
				mapTileCache.Remove(mapTileCache.Keys.First());
			}

			mapTileCache.Add(key, mapTile);

			return mapTile;
		}

		private sealed class MapTile
		{
			public readonly Model3DGroup Model3DGroup;

			public MapTile(Model3DGroup model3DGroup)
			{
				Model3DGroup = model3DGroup;
			}
		}
	}
}
