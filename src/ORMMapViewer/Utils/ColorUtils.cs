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
	}
}
