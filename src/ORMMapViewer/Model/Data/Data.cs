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

		public abstract double ConvertToMapZoom(double zoom);

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

			for (int i = 0; i < layer.FeatureCount(); i++)
			{
				VectorTileFeature feature = layer.GetFeature(i);
				List<Vector2<int>> geometry = feature.Geometry<int>()[0];

				for (int index = 1; index < geometry.Count; index++)
				{
					MaskDrawer.DrawMaskLine(geometry[index - 1], geometry[index]);
				}
			}

			roadsCache.Add(lonLatZoom.ToString(), MaskDrawer.GetGraph());


			/*Graph graph = new Graph();
			List<Line> lines = new List<Line>();

			for (int i = 0; i < layer.FeatureCount(); i++)
			{
				VectorTileFeature feature = layer.GetFeature(i);
				List<Vector2<int>> geometry = feature.Geometry<int>()[0];

				Node last = null;
				for (int k = 0; k < geometry.Count; k++)
				{
					Node current = new Node(geometry[k].X, geometry[k].Y);
					if (current.pos.X > 4500 || current.pos.Y > 4500)
					{
						last = null;
						continue;
					}

					current = graph.AddNode(current);
					if (last != null)
					{
						Graph.LinkNodes(last, current);
						lines.Add(new Line(current, last));
					}

					last = current;
				}
			}

			Line[] copyOfLines = new Line[lines.Count];
			lines.CopyTo(copyOfLines);

			foreach (Line line in copyOfLines)
			{
				foreach (Node node in graph.nodes)
				{
					if (line.IsVectorOnLine(node.pos))
					{
						Graph.UnlinkNodes(line.startNode, line.endNode);
						Graph.LinkNodes(line.startNode, node);
						Graph.LinkNodes(line.endNode, node);

						lines.Remove(line);
						lines.Add(new Line(line.startNode, node));
						lines.Add(new Line(line.endNode, node));
					}
				}
			}

			foreach (Line line1 in lines)
			{
				foreach (Line line2 in lines)
				{
					if (line1 != line2)
					{
						graph.CheckAndAddIntersection(line1, line2);
					}
				}
			}


			// Console.WriteLine(string.Join(", ", graph.nodes[123].neighbours.Select(node=>node.Key.id)));

			Console.WriteLine(new Line(graph.nodes[8], graph.nodes[42]).IsVectorOnLine(graph.nodes[100].pos));

			// graph.CheckAndAddIntersection(graph.nodes[309], graph.nodes[310], graph.nodes[76], graph.nodes[75]);

			foreach (Node node in graph.nodes)
			{
				node.UpdateNeighbours();
			}

			roadsCache.Add(lonLatZoom.ToString(), graph);*/


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
