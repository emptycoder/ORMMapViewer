using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using ORMMap;
using ORMMap.Model.Data;
using ORMMap.Model.Entitites;
using ORMMapViewer.Utils;

namespace ORMMapViewer
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static readonly Dictionary<string, Pallete> drawingLayersPallete = new Dictionary<string, Pallete>
		{
			{"landuse", new Pallete(ColorUtils.GetColor("#7fdf7f"), ColorUtils.GetColor("#7fdf7f"), 1)},
			{"earth", new Pallete(ColorUtils.GetColor("#2c2c2c"), ColorUtils.GetColor("#2c2c2c"), 1)},
			{"water", new Pallete(ColorUtils.GetColor("#7676D0"), ColorUtils.GetColor("#8F8FE7"), 2)},
			{"places", new Pallete(Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255), 1)},
			{"boundaries", new Pallete(Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255), 1)},
			{"pois", new Pallete(Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), 1)},
			{"roads", new Pallete(ColorUtils.GetColor("#cccccc"), ColorUtils.GetColor("#cccccc"), 1)},
			{"transit", new Pallete(Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255), 1)}
		};

		private static readonly Dictionary<string, Pallete> modelsLayersPallete = new Dictionary<string, Pallete>
		{
			{"buildings", new Pallete(ColorUtils.GetColor("#7f7f7f"), ColorUtils.GetColor("#7f7f7f"), 1)}
		};
		private Data dataController = new TangramData(Environment.CurrentDirectory + "\\data");


		public MainWindow()
		{
			InitializeComponent();
			InitializeProjection();
			Title = $"ORMMap [Zoom: {zoom}]";
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) => UpdateScene(dataController.GetTileSize(dataController.ConvertToMapZoom(Settings.zoom)));
	}
}
