using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ORMMap.Model.Entitites;
using ORMMap.VectorTile;
using ORMMap.VectorTile.Geometry;
using ORMMapViewer.Model.Entitites;
using ORMMapViewer.Utils;
using ORMMapViewer.Utils.JsonConverters;

namespace ORMMap.Model.Data
{
	/// <summary>
	/// Data cache layer.
	/// </summary>
	public abstract class Data
	{
		private static JsonSerializerSettings settings = new JsonSerializerSettings()
		{
			ContractResolver = new DictionaryAsArrayResolver(),
			ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
			PreserveReferencesHandling = PreserveReferencesHandling.Objects,
			TypeNameHandling = TypeNameHandling.Auto,
			NullValueHandling = NullValueHandling.Ignore
		};

		private readonly Dictionary<string, string> diskMapCache;
		private readonly Dictionary<string, string> diskRoadCache;

		private readonly Dictionary<string, VectorTileObj> memoryMapCache =
			new Dictionary<string, VectorTileObj>(Settings.memoryDataCacheOpacity);
		private readonly Dictionary<string, Graph> memoryRoadsCache = new Dictionary<string, Graph>();

		protected readonly string pathToDataFolder;

		/// <summary>
		/// Folder hierarchy:
		/// - {pathToDataFolder}
		/// - {pathToDataFolder}/roads
		/// - {pathToDataFolder}/tiles
		/// 
		/// Data cache layers:
		/// - Map cache (memory, disk)
		/// - Road cache (memory, disk)
		/// </summary>
		/// <param name="pathToDataFolder">Path to data folder.</param>
		public Data(string pathToDataFolder)
		{
			this.pathToDataFolder = pathToDataFolder + "\\" + MethodName;
			// Check folders existing
			DirectoryUtils.TryCreateFolder(this.pathToDataFolder);
			DirectoryUtils.TryCreateFolder(this.pathToDataFolder + "\\roads");
			DirectoryUtils.TryCreateFolder(this.pathToDataFolder + "\\tiles");
			// Scan folder for cache files
			string[] mapPathes = Directory.GetFiles(this.pathToDataFolder + "\\tiles", $"*{MapFileExtension}", SearchOption.AllDirectories);
			string[] roadPathes = Directory.GetFiles(this.pathToDataFolder + "\\roads", $"*{RoadFileExtension}", SearchOption.AllDirectories);
			diskMapCache = mapPathes.ToDictionary(
				path => Vector3<double>.DecodeFromJSON(Path.GetFileNameWithoutExtension(path)).ToString(),
				path => path);
			diskRoadCache = roadPathes.ToDictionary(
				path => Vector3<double>.DecodeFromJSON(Path.GetFileNameWithoutExtension(path)).ToString(),
				path => path);
		}

		public abstract string MethodName { get; }
		protected abstract string MapFileExtension { get; }
		protected abstract string RoadFileExtension { get; }

		public abstract int GetTileSize(double zoom);

		public abstract int ConvertToMapZoom(double zoom);

		protected abstract byte[] GetDataFromSource(Vector3<double> lonLatZoom);

		public VectorTileObj GetData(Vector3<double> lonLatZoom) // throws WebException
		{
			if (memoryMapCache.TryGetValue(lonLatZoom.ToString(), out VectorTileObj data))
			{
				return data;
			}
			else if (diskMapCache.TryGetValue(lonLatZoom.ToString(), out string path))
			{
				data = new VectorTileObj(File.ReadAllBytes(path));
			}
			else
			{
				byte[] byteData = GetDataFromSource(lonLatZoom);
				// Save to disk
				path = $"{pathToDataFolder}\\tiles\\{lonLatZoom.EncodeToString()}{MapFileExtension}";
				File.WriteAllBytes(path, byteData);
				// Add to disk cache
				diskMapCache.Add(lonLatZoom.ToString(), path);

				data = new VectorTileObj(byteData);
			}

			CacheMapToMemory(lonLatZoom.ToString(), data);
			CacheRoads(lonLatZoom, data);

			return data;
		}

		public Graph GetRoads(Vector3<double> lonLatZoom)
		{
			if (memoryRoadsCache.TryGetValue(lonLatZoom.ToString(), out Graph graph))
			{
				return graph;
			}

			if (diskRoadCache.TryGetValue(lonLatZoom.ToString(), out string path))
			{
				graph = JsonConvert.DeserializeObject<Graph>(File.ReadAllText(path), settings);
				memoryRoadsCache.Add(lonLatZoom.ToString(), graph);
				return graph;
			}

			// Try get data and then return roads
			// If path finding need tile which wasn't shown
			// GetData(lonLatZoom);

			if (memoryRoadsCache.TryGetValue(lonLatZoom.ToString(), out graph))
			{
				return graph;
			}

			return null;
		}

		// Need check memory and disk existing because we can load road graph for path finding
		private void CacheRoads(Vector3<double> lonLatZoom, VectorTileObj data)
		{
			if (memoryRoadsCache.ContainsKey(lonLatZoom.ToString()))
			{
				return;
			}

			if (diskRoadCache.TryGetValue(lonLatZoom.ToString(), out string path))
			{
				memoryRoadsCache.Add(lonLatZoom.ToString(), JsonConvert.DeserializeObject<Graph>(File.ReadAllText(path), settings));
				return;
			}

			Graph graph;
			if (data.LayerNames().Contains("roads"))
			{
				VectorTileLayer layer = data.GetLayer("roads");
				MaskGraphCreator maskDrawer = new MaskGraphCreator(GetTileSize(ConvertToMapZoom(Settings.zoom)));

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
				graph = maskDrawer.GetGraph();
			}
			else
			{
				graph = new Graph();
			}
			
			memoryRoadsCache.Add(lonLatZoom.ToString(), graph);
			// Save to disk
			path = $"{pathToDataFolder}\\roads\\{lonLatZoom.EncodeToString()}{RoadFileExtension}";
			File.WriteAllText(path, JsonConvert.SerializeObject(graph, settings));
			// Add to disk cache
			diskRoadCache.Add(lonLatZoom.ToString(), path);
		}

		private void CacheMapToMemory(string lonLatZoom, VectorTileObj data)
		{
			if (memoryMapCache.Count == Settings.memoryDataCacheOpacity)
			{
				memoryMapCache.Remove(memoryMapCache.Keys.First());
			}

			memoryMapCache.Add(lonLatZoom, data);
		}
	}
}
