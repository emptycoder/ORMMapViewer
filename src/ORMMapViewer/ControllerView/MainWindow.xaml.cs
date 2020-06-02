using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using ORMMap;
using ORMMap.Model.Data;
using ORMMap.Model.Entitites;
using ORMMap.VectorTile;
using ORMMap.VectorTile.Geometry;
using ORMMapViewer.Model.Entitites;
using ORMMapViewer.Utils;

namespace ORMMapViewer
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static readonly Dictionary<string, Pallete> layersPallete = new Dictionary<string, Pallete>
		{
			{"landuse", new Pallete(ColorUtils.GetColor("#7fdf7f"), ColorUtils.GetColor("#7fdf7f"), 1)},
			{"earth", new Pallete(ColorUtils.GetColor("#2c2c2c"), ColorUtils.GetColor("#2c2c2c"), 1)},
			{"water", new Pallete(ColorUtils.GetColor("#7676D0"), ColorUtils.GetColor("#8F8FE7"), 10)},
			{"places", new Pallete(Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255), 1)},
			{"boundaries", new Pallete(Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255), 1)},
			{"pois", new Pallete(Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), 1)},
			{"buildings", new Pallete(ColorUtils.GetColor("#7f7f7f"), ColorUtils.GetColor("#7f7f7f"), 1)},
			{"roads", new Pallete(ColorUtils.GetColor("#cccccc"), ColorUtils.GetColor("#cccccc"), 16)},
			{"transit", new Pallete(Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255), 1)}
		};

		private readonly LatLng nowCoordinations = Settings.startPosition;

		private Data dataController;

		public MainWindow()
		{
			InitializeComponent();
			Title = $"ORMMap [Zoom: {zoom}]";
			InitializeScene();
		}

		private void InitializeScene()
		{
			dataController = new TangramData(Environment.CurrentDirectory + "\\data");
		}

		private void UpdateScene()
		{
			double cZoom = dataController.ConvertToMapZoom(zoom);
			Vector2<uint> tileCoordinations = MercatorProjection.LatLngToTile(nowCoordinations, cZoom);
			Vector3<double> lonLatZoom = new Vector3<double>(
				tileCoordinations.X,
				tileCoordinations.Y,
				cZoom
			);
			VectorTileObj tile = dataController.GetData(lonLatZoom);

			Bitmap scene = new Bitmap((int) dataController.GetTileScale(), (int) dataController.GetTileScale());
			using (Graphics graphics = Graphics.FromImage(scene))
			{
				ReadOnlyCollection<string> layers = tile.LayerNames();
				foreach (string layerName in layersPallete.Keys)
				{
					if (layers.Contains(layerName))
					{
						VectorTileLayer layer = tile.GetLayer(layerName);
						MVTDrawer.DrawLayer(layer, layersPallete[layerName], graphics);
					}
				}

				Graph graph = dataController.GetRoads(lonLatZoom);
				LinkedList<Node> list = AStarPathSearch.FindPath(graph.nodes[423], graph.nodes[83]);
				if (list != null && list.Count > 1)
				{
					MVTDrawer.DrawGraphRoads(list, graphics);
				}
				MVTDrawer.DrawNodeIndices(graph.nodes, graphics);
			}

			map.ImageSource = ImageUtils.GetImageStream(scene);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			UpdateScene();
		}
	}
}
