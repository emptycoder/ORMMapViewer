using System.Drawing;

namespace ORMMapViewer.Utils
{
	public static class ColorUtils
	{
		public static string GetHex(this Color color)
		{
			return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
		}

		public static Color GetColor(string hexValue)
		{
			return ColorTranslator.FromHtml(hexValue);
		}

		public static System.Windows.Media.Color ToMediaColor(this Color color)
		{
			return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
		}
	}
}
