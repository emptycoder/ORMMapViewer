using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ORMMap.Model.Entitites;
using ORMMap.VectorTile;
using ORMMap.VectorTile.Geometry;
using ORMMapViewer.Model.Entitites;
using ORMMapViewer.Utils;

namespace ORMMap.Model.Data
{
	public abstract class Data
	{
		private readonly Dictionary<string, string> diskCache;

		private readonly Dictionary<string, VectorTileObj> memoryCache =
			new Dictionary<string, VectorTileObj>(Settings.memoryCacheOpacity);

		protected readonly string pathToDataFolder;

		private readonly Dictionary<string, Graph> roadsCache = new Dictionary<string, Graph>();

		public Data(string pathToDataFolder)
		{
			this.pathToDataFolder = pathToDataFolder + "//" + MethodName;
			// Check for folder existing
			DirectoryUtils.TryCreateFolder(this.pathToDataFolder);
			// Scan folder for cache files
			string[] pathes = Directory.GetFiles(this.pathToDataFolder, $"*{FileExtension}", SearchOption.AllDirectories);
			diskCache = pathes.ToDictionary(
				path => Vector3<double>.DecodeFromJSON(Path.GetFileNameWithoutExtension(path)).ToString(),
				path => path);
		}

		public abstract string MethodName { get; }
		protected abstract string FileExtension { get; }

		public abstract uint GetTileScale();

		public abstract uint GetTileSize();

		public abstract int ConvertToMapZoom(double zoom);

		protected abstract byte[] GetDataFromSource(Vector3<double> lonLatZoom);

		public VectorTileObj GetData(Vector3<double> lonLatZoom)
		{
			if (memoryCache.TryGetValue(lonLatZoom.ToString(), out VectorTileObj data))
			{
				return data;
			}

			if (diskCache.TryGetValue(lonLatZoom.ToString(), out string path))
			{
				data = new VectorTileObj(File.ReadAllBytes(path));
			}
			else
			{
				byte[] byteData = GetDataFromSource(lonLatZoom);
				// Save to disk
				path = $"{pathToDataFolder}\\{lonLatZoom.EncodeToString()}{FileExtension}";
				File.WriteAllBytes(path, byteData);
				// Add to disk cache
				diskCache.Add(lonLatZoom.ToString(), path);

				data = new VectorTileObj(byteData);
			}

			CacheToMemory(lonLatZoom, data);
			CacheRoads(lonLatZoom, data);

			return data;
		}

		public Graph GetRoads(Vector3<double> lonLatZoom)
		{
			return roadsCache[lonLatZoom.ToString()];
		}

		private void CacheRoads(Vector3<double> lonLatZoom, VectorTileObj data)
		{
			if (roadsCache.ContainsKey(lonLatZoom.ToString()))
			{
				return;
			}

			if (!data.LayerNames().Contains("roads"))
			{
				return;
			}

			VectorTileLayer layer = data.GetLayer("roads");
			MaskDrawer maskDrawer = new MaskDrawer();

			for (int i = 0; i < layer.FeatureCount(); i++)
			{
				VectorTileFeature feature = layer.GetFeature(i);
				foreach (List<Vector2<int>> geometry in feature.Geometry<int>())
				{
					for (int index = 1; index < geometry.Count; index++)
					{
						maskDrawer.DrawMaskLine(geometry[index - 1], geometry[index]);
					}
				}
			}

			roadsCache.Add(lonLatZoom.ToString(), maskDrawer.GetGraph());
		}

		private void CacheToMemory(Vector3<double> lonLatZoom, VectorTileObj data)
		{
			if (memoryCache.Count == Settings.memoryCacheOpacity)
			{
				memoryCache.Remove(memoryCache.Keys.First());
			}

			memoryCache.Add(lonLatZoom.ToString(), data);
		}
	}
}
