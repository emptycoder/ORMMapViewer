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

namespace ORMMapViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LatLng nowCoordinations = Settings.startPosition;
        Data dataController;

        private static Dictionary<string, Pallete> layersPallete = new Dictionary<string, Pallete>()
        {
            { "landuse", new Pallete(ColorUtils.GetColor("#7fdf7f"), ColorUtils.GetColor("#7fdf7f"), 1) },
            { "earth", new Pallete(ColorUtils.GetColor("#2c2c2c"), ColorUtils.GetColor("#2c2c2c"), 1) },
            { "water", new Pallete(ColorUtils.GetColor("#7676D0"), ColorUtils.GetColor("#8F8FE7"), 10) },
            { "places", new Pallete(Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255), 1) },
            { "transit", new Pallete(Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255), 1) },
            { "boundaries", new Pallete(Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255), 1) },
            { "pois", new Pallete(Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), 1) },
            { "buildings", new Pallete(ColorUtils.GetColor("#7f7f7f"), ColorUtils.GetColor("#7f7f7f"), 1) },
            { "roads", new Pallete(ColorUtils.GetColor("#cccccc"), ColorUtils.GetColor("#cccccc"), 20) }
        };

        public MainWindow()
        {
            InitializeComponent();
            this.Title = $"ORMMap [Zoom: {zoom}]";
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
            VectorTileObj tile = dataController.GetData(new Vector3<double>(
                tileCoordinations.X,
                tileCoordinations.Y,
                cZoom
            ));

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

            map.ImageSource = ImageUtils.GetImageStream(scene);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateScene();
        }
    }
}
