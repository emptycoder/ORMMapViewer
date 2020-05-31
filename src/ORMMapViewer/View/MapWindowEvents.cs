using ORMMap;
using ORMMap.VectorTile.Geometry;
using System;
using System.Windows;

namespace ORMMapViewer
{
    public partial class MainWindow
    {
        double zoom = Settings.zoom;
        bool mouseDown;
        Vector2<int> oldPos = new Vector2<int>(0, 0);

        private void Window_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            zoom += e.Delta / (50 * zoom);
            zoom = Math.Min(Math.Max(zoom, 1), 20);
            camera.Position = new System.Windows.Media.Media3D.Point3D(camera.Position.X, camera.Position.Y, 5041.23 * (21 - zoom));
            this.Title = $"ORMMap [Zoom: {zoom}]";

            //UpdateScene();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!mouseDown)
            {
                Point pos = e.GetPosition(this);
                oldPos.set((int)pos.X, (int)pos.Y);
                mouseDown = true;
            }
        }

        private void Window_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) => mouseDown = false;

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!mouseDown) return;
            
            Point pos = e.GetPosition(this);
            Console.WriteLine(pos + ", " + oldPos);
            camera.Position = new System.Windows.Media.Media3D.Point3D(camera.Position.X + (pos.X - oldPos.X)*20, camera.Position.Y + (pos.Y - oldPos.Y)*20, camera.Position.Z);
            Console.WriteLine((pos.X - oldPos.X) + ", " + (pos.Y - oldPos.Y));
            oldPos.set((int) pos.X, (int) pos.Y);
        }
    }
}
