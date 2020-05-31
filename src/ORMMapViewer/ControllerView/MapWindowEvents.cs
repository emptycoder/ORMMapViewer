﻿using ORMMap;
using ORMMap.VectorTile.Geometry;
using System;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Input;

namespace ORMMapViewer
{
    public partial class MainWindow
    {
        double zoom = Settings.zoom;
        bool mouseDown;
        Vector2<int> oldPos = new Vector2<int>(0, 0);

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoom += e.Delta / (50 * zoom);
            zoom = Math.Min(Math.Max(zoom, 1), 20);
            camera.Position = new Point3D(camera.Position.X, camera.Position.Y, 5041.23 * (21 - zoom));
            this.Title = $"ORMMap [Zoom: {zoom}]";

            //UpdateScene();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!mouseDown)
            {
                Point pos = e.GetPosition(this);
                oldPos.set((int)pos.X, (int)pos.Y);
                mouseDown = true;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e) => mouseDown = false;

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown) return;
            
            Point pos = e.GetPosition(this);
            camera.Position = new Point3D(camera.Position.X - (pos.X - oldPos.X)*(200/zoom), camera.Position.Y + (pos.Y - oldPos.Y)*(200/zoom), camera.Position.Z);
            oldPos.set((int) pos.X, (int) pos.Y);
        }
    }
}