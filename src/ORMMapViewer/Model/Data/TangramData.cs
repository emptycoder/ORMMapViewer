using System;
using System.Net;
using ORMMap.Model.Entitites;

namespace ORMMap.Model.Data
{
	public class TangramData : Data
	{
		private const string apiUrl = "https://tile.nextzen.org/tilezen/vector/v1/512/all";
		private const string apiKey = "PkX5Y0U9RSK0j50FhRh_fQ";

		public TangramData(string pathToDataFolder) : base(pathToDataFolder)
		{
		}

		public override string MethodName => "tangramData";

		protected override string FileExtension => ".mvt";

		public override uint GetTileSize()
		{
			return 256;
		}

		public override uint GetTileScale()
		{
			return 4096;
		}

		// mapZoomLevels from 1 to 16 inclusive
		public override double ConvertToMapZoom(double zoom)
		{
			return Math.Min(16, Math.Floor(zoom));
		}

		// {z}/{x}/{y}
		// 16/19376/24637
		protected override byte[] GetDataFromSource(Vector3<double> lonLatZoom)
		{
			WebClient webClient = new WebClient();
			return webClient.DownloadData(
				$"{apiUrl}/{lonLatZoom.Z}/{lonLatZoom.X}/{lonLatZoom.Y}{FileExtension}?api_key={apiKey}");
		}
	}
}
