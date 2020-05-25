using ORMMap.Model.Entitites;
using System;

namespace ORMMap.Model.Data
{
    public class TangramData : IData
    {
        public byte[] GetData(Vector3<double> lonLatZoom)
        {
            throw new NotImplementedException();
        }

        public uint GetTileSize()
        {
            return 4096;
        }
    }
}
