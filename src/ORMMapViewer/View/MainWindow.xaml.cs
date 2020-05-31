using ORMMap;
using ORMMap.Model.Data;
using ORMMap.Model.Entitites;
using ORMMap.VectorTile;
using ORMMap.VectorTile.Geometry;
using ORMMapViewer.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ORMMapViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LatLng nowCoordinations = Settings.startPosition;
        MercatorProjection mercatorProjection;
        Data dataController;
        //VectorTileObj[,] scene;

        private static Dictionary<string, Pallete> layersPallete = new Dictionary<string, Pallete>()
        {
            { "landuse", new Pallete(ColorUtils.GetColor("#7fdf7f"), ColorUtils.GetColor("#7fdf7f"), 1) },
            { "earth", new Pallete(ColorUtils.GetColor("#2c2c2c"), ColorUtils.GetColor("#2c2c2c"), 1) },
            { "water", new Pallete(ColorUtils.GetColor("#7676D0"), ColorUtils.GetColor("#8F8FE7"), 10) },
            { "places", new Pallete(Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255), 1) },
            { "transit", new Pallete(Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255), 1) },
            { "boundaries", new Pallete(Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255), 1) },
            { "roads", new Pallete(ColorUtils.GetColor("#cccccc"), ColorUtils.GetColor("#cccccc"), 20) },
            { "pois", new Pallete(Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), 1) },
            { "buildings", new Pallete(ColorUtils.GetColor("#7f7f7f"), ColorUtils.GetColor("#7f7f7f"), 1) }
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
            dataController = new TangramData(Environment.CurrentDirectory + "\\data");
            mercatorProjection = new MercatorProjection(dataController.GetTileSize(), dataController.GetTileScale());
        }

        private void UpdateScene()
        {
            Vector2<uint> tileCoordinations = mercatorProjection.LatLngToTile(nowCoordinations, zoom);
            VectorTileObj tile = new VectorTileObj(dataController.GetData(new Vector3<double>(
                tileCoordinations.X,
                tileCoordinations.Y,
                zoom))
            );

            Bitmap scene = new Bitmap((int)dataController.GetTileScale(), (int)dataController.GetTileScale());
            using (Graphics graphics = Graphics.FromImage(scene))
            {
                var layers = tile.LayerNames();
                foreach (string layerName in layersPallete.Keys)
                {
                    if (layers.Contains(layerName))
                    {
                        VectorTileLayer layer = tile.GetLayer(layerName);
                        Console.WriteLine(layerName);
                        MVTDrawer.DrawLayer(layer, layersPallete[layerName], graphics);
                    }
                }
            }

            sceneControl.Source = ImageUtils.GetImageStream(scene);
        }

        private void ZoomMap(int zoom)
        {
            /* Image img = sceneControl.BackgroundImage;
            sceneControl.BackgroundImage = new Bitmap(img, img.Width * zoom, img.Height * zoom);
            img?.Dispose();*/

            this.Title = $"ORMMap [Zoom: {zoom}]";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateScene();
        }
    }
}
