using ORMMap.Model.Data;
using ORMMap.Model.Entitites;
using ORMMap.VectorTile;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ORMMap
{
	public partial class MapForm : Form
    {
		Vector2<uint> visionArea = Settings.startPosition;
		MercatorProjection mercatorProjection;
		IData dataController;
		//VectorTileObj[,] scene;

		private static Dictionary<string, Pallete> layersPallete = new Dictionary<string, Pallete>()
		{
			{ "landuse", new Pallete(Color.Yellow, Color.Yellow) },
			{ "earth", new Pallete(Color.Green, Color.Green) },
			{ "roads", new Pallete(Color.Gray, Color.Gray) },
			{ "pois", new Pallete(Color.Red, Color.Red) },
			{ "buildings", new Pallete(Color.Violet, Color.Violet) }
		};

		public MapForm()
        {
            InitializeComponent();
			this.Text = $"ORMMap [Zoom: {zoom}]";
			InitializeScene();

			this.MouseWheel += MapForm_MouseWheel;
		}

		private void InitializeScene()
		{
			//scene = new VectorTileObj[(Settings.renderDistanceX * 2) - 1, (Settings.renderDistanceY * 2) - 1];
			dataController = new MockupData();
			mercatorProjection = new MercatorProjection(dataController.GetTileSize(), Settings.zoom);
		}

		private void Form1_Load(object sender, EventArgs e)
        {
			UpdateScene();
		}

		private void UpdateScene()
		{
			VectorTileObj tile = new VectorTileObj(dataController.GetData(new Vector3<double>(
				mercatorProjection.getLongitudeFromX(visionArea.X),
				mercatorProjection.getLatitudeFromY(visionArea.Y),
				mercatorProjection.Zoom))
			);

			Image scene = new Bitmap((int) dataController.GetTileSize(), (int) dataController.GetTileSize());
			using (Graphics graphics = Graphics.FromImage(scene))
			{
				var layerNames = tile.LayerNames();
				for (int i = tile.LayerNames().Count - 1; i >= 0; i--)
				{
					VectorTileLayer layer = tile.GetLayer(layerNames[i]);
					Console.WriteLine(layerNames[i]);
					MVTDrawer.DrawLayer(layer, layersPallete[layerNames[i]], graphics);
				}
			}

			sceneControl.BackgroundImage?.Dispose();
			sceneControl.BackgroundImage = scene;
		}

		private void ZoomMap(int zoom)
		{
			Image img = sceneControl.BackgroundImage;
			sceneControl.BackgroundImage = new Bitmap(img, img.Width * zoom, img.Height * zoom);
			img?.Dispose();

			this.Text = $"ORMMap [Zoom: {zoom}]";
		}
	}
}
