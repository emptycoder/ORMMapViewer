using ORMMap.Model.Entitites;
using ORMMap.VectorTile;
using ORMMapViewer.Model.Entitites;
using ORMMapViewer.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ORMMap.Model.Data
{
    public abstract class Data
    {
        public abstract string MethodName { get; }
        protected abstract string FileExtension { get; }

        private Dictionary<string, string> diskCache;
        private Dictionary<string, VectorTileObj> memoryCache = new Dictionary<string, VectorTileObj>(Settings.memoryCacheOpacity);
        private Dictionary<string, Graph> roadsCache = new Dictionary<string, Graph>();

        protected readonly string pathToDataFolder;

        public Data(string pathToDataFolder)
        {
            this.pathToDataFolder = pathToDataFolder + "//" + MethodName;
            // Check for folder existing
            DirectoryUtils.TryCreateFolder(this.pathToDataFolder);
            // Scan folder for cache files
            var pathes = Directory.GetFiles(this.pathToDataFolder, $"*{FileExtension}", SearchOption.AllDirectories);
            diskCache = pathes.ToDictionary((path) => Vector3<double>.DecodeFromJSON(Path.GetFileNameWithoutExtension(path)).ToString(), (path) => path);
        }

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

        private void CacheRoads(Vector3<double> lonLatZoom, VectorTileObj data)
        {
            if (roadsCache.ContainsKey(lonLatZoom.ToString())) { return; }
            if (!data.LayerNames().Contains("roads"))
            {
                return;
            }
            var layer = data.GetLayer("roads");

            Graph graph = new Graph();

            for (int i = 0; i < layer.FeatureCount(); i++)
            {
                VectorTileFeature feature = layer.GetFeature(i);
                var geometry = feature.Geometry<int>()[0];

                Node node1 = new Node(geometry[0].X, geometry[0].Y);
                Node node2 = new Node(geometry[1].X, geometry[1].Y);

                node1.relatives.Add(node2, new LengthWeight(node1, node2));
                node2.relatives.Add(node1, new LengthWeight(node2, node1));

                graph.AddNode(node1);
                graph.AddNode(node2);
            }

            foreach (Node node in graph.nodes)
            {
                node.UpdateRelatives();
            }

            roadsCache.Add(lonLatZoom.ToString(), graph);
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
