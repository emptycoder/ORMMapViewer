using System;
using System.Net;
using ORMMap.Model.Entitites;

namespace ORMMap.Model.Data
{
	public class TangramData : Data
	{
		private const string apiUrl = "https://tile.nextzen.org/tilezen/vector/v1/512/all";
		private const string apiKey = "PkX5Y0U9RSK0j50FhRh_fQ";

		public TangramData(string pathToDataFolder) : base(pathToDataFolder) { }

		public override string MethodName => "tangramData";

		protected override string MapFileExtension => ".mvt";

		protected override string RoadFileExtension => ".road";

		public override int GetTileSize(double zoom)
		{
			return 4096;
		}

		// mapZoomLevels from 1 to 15 inclusive
		public override int ConvertToMapZoom(double zoom)
		{
			return (int)Math.Min(15, Math.Floor(zoom));
		}

		// {z}/{x}/{y}
		// 16/19376/24637
		protected override byte[] GetDataFromSource(Vector3<double> lonLatZoom)
		{
			WebClient webClient = new WebClient();
			return webClient.DownloadData(
				$"{apiUrl}/{lonLatZoom.Z}/{lonLatZoom.X}/{lonLatZoom.Y}{MapFileExtension}?api_key={apiKey}");
		}
	}
}
