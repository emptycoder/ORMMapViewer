using System;
using System.Windows.Forms;

namespace ORMMap
{
    public partial class MapForm : Form
    {
        int zoom = 1;

        private void MapForm_MouseWheel(object sender, MouseEventArgs e)
        {
            zoom += e.Delta * SystemInformation.MouseWheelScrollLines / 120;

            if (zoom < 1) { zoom = 1; }
            else if (zoom > 4) { zoom = 4; }

            // ZoomMap(zoom);
        }
    }
}
