using System.IO;

namespace ORMMapViewer.Utils
{
	public static class DirectoryUtils
	{
		public static bool TryCreateFolder(string path)
		{
			if (Directory.Exists(path)) return false;

			Directory.CreateDirectory(path);
			return true;
		}
	}
}
