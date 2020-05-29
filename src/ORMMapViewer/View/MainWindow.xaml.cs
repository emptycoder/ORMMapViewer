using ORMMap;
using ORMMap.Model.Data;
using ORMMap.Model.Entitites;
using ORMMap.VectorTile;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ORMMapViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Vector2<uint> visionArea = Settings.startPosition;
        MercatorProjection mercatorProjection;
        Data dataController;
        //VectorTileObj[,] scene;

        // TODO: Add water to layers
        /*private static Dictionary<string, Pallete> layersPallete = new Dictionary<string, Pallete>()
        {
            { "landuse", new Pallete(Color.Yellow, Color.Yellow) },
            { "earth", new Pallete(Color.Green, Color.Green) },
            { "roads", new Pallete(Color.Gray, Color.Gray) },
            { "pois", new Pallete(Color.Red, Color.Red) },
            { "buildings", new Pallete(Color.Violet, Color.Violet) }
        };*/

        public MainWindow()
        {
            InitializeComponent();
            this.Title = $"ORMMap [Zoom: {zoom}]";
            InitializeScene();
        }

        private void InitializeScene()
        {
            //scene = new VectorTileObj[(Settings.renderDistanceX * 2) - 1, (Settings.renderDistanceY * 2) - 1];
            dataController = new MockupData(Environment.CurrentDirectory + "\\data");
            mercatorProjection = new MercatorProjection(dataController.GetTileSize(), Settings.zoom);
        }

        private void UpdateScene()
        {
            VectorTileObj tile = new VectorTileObj(dataController.GetData(new Vector3<double>(
                mercatorProjection.getLongitudeFromX(visionArea.X),
                mercatorProjection.getLatitudeFromY(visionArea.Y),
                mercatorProjection.Zoom))
            );

            var layerNames = tile.LayerNames();
            DrawingGroup drawingGroup = new DrawingGroup();
            for (int i = tile.LayerNames().Count - 1; i >= 0; i--)
            {
                Console.WriteLine(layerNames[i]);
                VectorTileLayer layer = tile.GetLayer(layerNames[i]);
                var geometryGroup = MVTDrawer.GetGeometryFromLayer(layer);
                drawingGroup.Children.Add(new GeometryDrawing(
                    new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    new Pen(new SolidColorBrush(Color.FromRgb(255, 255, 255)), 10),
                    geometryGroup)
                );
            }

            DrawingImage geometryImage = new DrawingImage(drawingGroup);
            sceneControl.Source = geometryImage;
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
