using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ORMMapViewer.Utils
{
	public static class ImageUtils
	{
		public static BitmapSource GetImageStream(Image myImage)
		{
			Bitmap bitmap = new Bitmap(myImage);
			IntPtr bmpPt = bitmap.GetHbitmap();
			BitmapSource bitmapSource =
				Imaging.CreateBitmapSourceFromHBitmap(
					bmpPt,
					IntPtr.Zero,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());

			//freeze bitmapSource and clear memory to avoid memory leaks
			bitmapSource.Freeze();
			DeleteObject(bmpPt);

			return bitmapSource;
		}

		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DeleteObject(IntPtr value);
	}
}
