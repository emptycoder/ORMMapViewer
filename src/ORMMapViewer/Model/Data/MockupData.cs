using System;
using ORMMap.Model.Entitites;
using ORMMapViewer.Properties;

namespace ORMMap.Model.Data
{
	public class MockupData : Data
	{
		public MockupData(string pathToDataFolder) : base(pathToDataFolder)
		{
		}

		public override string MethodName => "mockupData";

		protected override string FileExtension => ".mvt";

		public override int GetTileSize(double zoom)
		{
			return 4096;
		}

		public override int ConvertToMapZoom(double zoom)
		{
			return 13;
		}

		protected override byte[] GetDataFromSource(Vector3<double> lonLatZoom)
		{
			Console.WriteLine(lonLatZoom.ToString());
			return Resources.example;
		}
	}
}
