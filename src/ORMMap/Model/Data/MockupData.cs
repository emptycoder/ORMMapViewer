using ORMMap.Model.Entitites;
using System.IO;

namespace ORMMap.Model.Data
{
    public class MockupData : IData
    {
        // {z}/{x}/{y}
        // 16/19376/24637
        public byte[] GetData(Vector3<double> lonLatZoom)
        {
            return File.ReadAllBytes("C:\\Users\\Yaroslav\\Desktop\\24637.mvt");
        }

        public uint GetTileSize()
        {
            return 4096;
        }
    }
}
