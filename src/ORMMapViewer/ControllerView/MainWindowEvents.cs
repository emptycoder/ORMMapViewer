using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using ORMMap;
using ORMMap.VectorTile.Geometry;

namespace ORMMapViewer
{
	public partial class MainWindow
	{
		private bool mouseDown;
		private Vector2<int> oldPos = new Vector2<int>(0, 0);
		private double zoom = Settings.zoom;
		

		private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			zoom += e.Delta / (50 * zoom);
			zoom = Math.Min(Math.Max(zoom, 1), 20);
			camera.Position = new Point3D(camera.Position.X, camera.Position.Y, 3692.3 * (21 - zoom));
			Title = $"ORMMap [Zoom: {zoom}]";

			// UpdateScene();
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!mouseDown)
			{
				Point pos = e.GetPosition(this);
				oldPos.Set((int) pos.X, (int) pos.Y);
				mouseDown = true;
			}
		}

		private void Window_MouseUp(object sender, MouseButtonEventArgs e)
		{
			mouseDown = false;
		}

		private void Window_MouseMove(object sender, MouseEventArgs e)
		{
			if (!mouseDown)
			{
				return;
			}

			Point pos = e.GetPosition(this);
			camera.Position = new Point3D(camera.Position.X - (pos.X - oldPos.X) * (200 / zoom),
				camera.Position.Y + (pos.Y - oldPos.Y) * (200 / zoom), camera.Position.Z);
			oldPos.Set((int) pos.X, (int) pos.Y);
		}
	}
}
