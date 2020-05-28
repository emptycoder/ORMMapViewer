using ORMMap;
using ORMMap.Model.Data;
using ORMMap.Model.Entitites;
using ORMMap.VectorTile;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace ORMMapViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Vector2<uint> visionArea = Settings.startPosition;
        MercatorProjection mercatorProjection;
        IData dataController;
        //VectorTileObj[,] scene;

        // TODO: Add water to layers
        private static Dictionary<string, Pallete> layersPallete = new Dictionary<string, Pallete>()
        {
            { "landuse", new Pallete(Color.Yellow, Color.Yellow) },
            { "earth", new Pallete(Color.Green, Color.Green) },
            { "roads", new Pallete(Color.Gray, Color.Gray) },
            { "pois", new Pallete(Color.Red, Color.Red) },
            { "buildings", new Pallete(Color.Violet, Color.Violet) }
        };

        public MainWindow()
        {
            InitializeComponent();
            this.Title = $"ORMMap [Zoom: {zoom}]";
            InitializeScene();
        }

        private void InitializeScene()
        {
            //scene = new VectorTileObj[(Settings.renderDistanceX * 2) - 1, (Settings.renderDistanceY * 2) - 1];
            dataController = new MockupData();
            mercatorProjection = new MercatorProjection(dataController.GetTileSize(), Settings.zoom);
        }

        private void UpdateScene()
        {
            VectorTileObj tile = new VectorTileObj(dataController.GetData(new Vector3<double>(
                mercatorProjection.getLongitudeFromX(visionArea.X),
                mercatorProjection.getLatitudeFromY(visionArea.Y),
                mercatorProjection.Zoom))
            );

            Image scene = new Bitmap((int)dataController.GetTileSize(), (int)dataController.GetTileSize());
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

            // TODO: watch for canvas api
            // sceneControl.BackgroundImage?.Dispose();
            // sceneControl.BackgroundImage = scene;
        }

        private void ZoomMap(int zoom)
        {
            /* Image img = sceneControl.BackgroundImage;
            sceneControl.BackgroundImage = new Bitmap(img, img.Width * zoom, img.Height * zoom);
            img?.Dispose();*/

            this.Title = $"ORMMap [Zoom: {zoom}]";
        }
    }
}
