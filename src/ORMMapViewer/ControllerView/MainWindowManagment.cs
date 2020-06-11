using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using ORMMap;
using ORMMap.VectorTile.Geometry;

namespace ORMMapViewer
{
	public partial class MainWindow
	{
		private bool leftMouseDown;
		private Vector2<int> oldPos = new Vector2<int>(0, 0);
		private double zoom = Settings.zoom;

		private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			zoom += e.Delta / (50 * zoom);
			zoom = Math.Min(Math.Max(zoom, 1), 20);
			camera.Position = new Point3D(camera.Position.X, camera.Position.Y, 3692.3 * (21 - zoom));
			Title = $"ORMMap [Zoom: {zoom}]";

			UpdateScene(dataController.GetTileSize(dataController.ConvertToMapZoom(zoom)));
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!leftMouseDown)
			{
				Point pos = e.GetPosition(this);
				oldPos.Set((int)pos.X, (int)pos.Y);
				leftMouseDown = true;
			}
		}

		private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			leftMouseDown = false;
			UpdateScene(dataController.GetTileSize(dataController.ConvertToMapZoom(zoom)));
		}

		private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			AddPointWithCheck(RayCastToMap(e.GetPosition(this)));
		}

		private void Window_MouseLeave(object sender, MouseEventArgs e)
		{
			leftMouseDown = false;
			UpdateScene(dataController.GetTileSize(dataController.ConvertToMapZoom(zoom)));
		}

		private void Window_MouseMove(object sender, MouseEventArgs e)
		{
			if (!leftMouseDown)
			{
				return;
			}

			Point pos = e.GetPosition(this);
			camera.Position = new Point3D(camera.Position.X - (oldPos.X - pos.X ) * (200 / zoom),
				camera.Position.Y + (oldPos.Y - pos.Y) * (200 / zoom), camera.Position.Z);
			oldPos.Set((int) pos.X, (int) pos.Y);
		}

		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			if (keyUpEvents.TryGetValue(e.Key, out Action action))
			{
				action.Invoke();
			}
		}
	}
}
