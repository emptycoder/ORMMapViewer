using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
		public static Vector3<int> cameraPos = new Vector3<int>(2048, 2048, 48000);
		public static Vector2<int> startPos = MercatorProjection.LatLngToTile(Settings.startPosition, Settings.zoom);
		
		public static Vector2<int> projectionSize = new Vector2<int>(10000, 10000);

		public void UpdateScene()
		{
			// tiles.Children.Clear();

			int tilesPerWidth = (int) Math.Ceiling((double) projectionSize.X / MapTile.size);
			int tilesPerHeight = (int) Math.Ceiling((double) projectionSize.Y / MapTile.size);
					Console.WriteLine(startPos);

			for (int x = 0; x < tilesPerWidth; x++)
			{
				for (int y = 0; y < tilesPerHeight; y++)
				{
					Vector2<int> localTilePos = new Vector2<int>(cameraPos.X - projectionSize.X / 2 + x * MapTile.size, cameraPos.Y - projectionSize.Y / 2 + y * MapTile.size);
					MapTile tile = new MapTile(localTilePos);
					Console.WriteLine(tile.hostPos);
					DrawTile(tile);
					tiles.Children.Add(tile.model);
				}
			}
		}

		public void DrawTile(MapTile tile)
		{
			double cZoom = dataController.ConvertToMapZoom(Settings.zoom);
			Vector3<double> lonLatZoom = new Vector3<double>(
				startPos.X,
				startPos.Y,
				cZoom
			);
			VectorTileObj vectorTileObj = dataController.GetData(lonLatZoom);

			Bitmap scene = new Bitmap((int) dataController.GetTileScale(), (int) dataController.GetTileScale());
			using (Graphics graphics = Graphics.FromImage(scene))
			{
				ReadOnlyCollection<string> layers = vectorTileObj.LayerNames();
				foreach (string layerName in layersPallete.Keys)
				{
					if (layers.Contains(layerName))
					{
						VectorTileLayer layer = vectorTileObj.GetLayer(layerName);
						MVTDrawer.DrawLayer(layer, layersPallete[layerName], graphics);
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

			tile.SetImageSource(ImageUtils.GetImageStream(scene));
		}

		public static Vector2<int> getTileHostPosFromLocalPos(Vector2<int> pos)
		{
			return pos.Divide(MapTile.size).Add(startPos);
		}
	}
}
