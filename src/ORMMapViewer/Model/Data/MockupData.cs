using ORMMap.Model.Entitites;
using System;

namespace ORMMap.Model.Data
{
    public class MockupData : Data
    {
        public override string MethodName => "mockupData";

        protected override string FileExtension => ".mvt";

        public MockupData(string pathToDataFolder) : base(pathToDataFolder) { }

        public override uint GetTileSize()
        {
            return 256;
        }

        public override uint GetTileScale()
        {
            return 4096;
        }

        public override double ConvertToMapZoom(double zoom)
        {
            return 13;
        }

        protected override byte[] GetDataFromSource(Vector3<double> lonLatZoom)
        {
            Console.WriteLine(lonLatZoom.ToString());
            return ORMMapViewer.Properties.Resources.example;
        }
    }
}
