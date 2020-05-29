using ORMMap.Model.Entitites;

namespace ORMMap.Model.Data
{
    public class MockupData : Data
    {
        protected override string FileExtension => ".mvt";

        public MockupData(string pathToDataFolder) : base(pathToDataFolder) { }

        public override uint GetTileSize()
        {
            return 4096;
        }

        protected override byte[] GetDataFromSource(Vector3<double> lonLatZoom)
        {
            return ORMMapViewer.Properties.Resources.example;
        }
    }
}
