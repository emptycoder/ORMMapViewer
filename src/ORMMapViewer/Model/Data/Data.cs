using ORMMap.Model.Entitites;
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

        private Dictionary<Vector3<double>, string> diskCache;
        private Dictionary<Vector3<double>, byte[]> memoryCache = new Dictionary<Vector3<double>, byte[]>(Settings.cacheOpacity);

        protected readonly string pathToDataFolder;

        public Data(string pathToDataFolder)
        {
            this.pathToDataFolder = pathToDataFolder + "//" + MethodName;
            // Check for folder existing
            DirectoryUtils.TryCreateFolder(this.pathToDataFolder);
            // Scan folder for cache files
            var pathes = Directory.GetFiles(this.pathToDataFolder, $"*{FileExtension}", SearchOption.AllDirectories);
            diskCache = pathes.ToDictionary((path) => Vector3<double>.DecodeFromString(Path.GetFileNameWithoutExtension(path)), (path) => path);
        }

        public abstract uint GetTileScale();

        public abstract uint GetTileSize();

        public byte[] GetData(Vector3<double> lonLatZoom)
        {
            if (memoryCache.TryGetValue(lonLatZoom, out byte[] data))
            {
                return data;
            }

            if (diskCache.TryGetValue(lonLatZoom, out string path))
            {
                data = File.ReadAllBytes(path);
                CacheToMemory(lonLatZoom, data);
            }

            data = GetDataFromSource(lonLatZoom);
            CacheToMemory(lonLatZoom, data);
            // Save to disk
            File.WriteAllBytes($"{pathToDataFolder}\\{lonLatZoom.EncodeToString()}{FileExtension}", data);

            return data;
        }

        private void CacheToMemory(Vector3<double> lonLatZoom, byte[] data)
        {
            if (memoryCache.Count == Settings.cacheOpacity)
            {
                memoryCache.Remove(memoryCache.Keys.First());
            }

            memoryCache.Add(lonLatZoom, data);
        }

        protected abstract byte[] GetDataFromSource(Vector3<double> lonLatZoom);
    }
}
