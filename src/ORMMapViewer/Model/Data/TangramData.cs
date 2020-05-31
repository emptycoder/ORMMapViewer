﻿using ORMMap.Model.Entitites;
using System.Net;
using System.Text;

namespace ORMMap.Model.Data
{
    public class TangramData : Data
    {
        public override string MethodName => "tangramData";

        protected override string FileExtension => ".mvt";

        const string apiUrl = "https://tile.nextzen.org/tilezen/vector/v1/512/all";
        const string apiKey = "PkX5Y0U9RSK0j50FhRh_fQ";

        public TangramData(string pathToDataFolder) : base(pathToDataFolder) {}

        public override uint GetTileSize()
        {
            return 256;
        }

        public override uint GetTileScale()
        {
            return 4096;
        }

        // {z}/{x}/{y}
        // 16/19376/24637
        protected override byte[] GetDataFromSource(Vector3<double> lonLatZoom)
        {
            WebClient webClient = new WebClient();
            return webClient.DownloadData($"{apiUrl}/{lonLatZoom.Z}/{lonLatZoom.X}/{lonLatZoom.Y}{FileExtension}?api_key={apiKey}");
        }
    }
}
