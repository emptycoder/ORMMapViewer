using ORMMap.Model.Entitites;

namespace ORMMap.Model.Data
{
    public interface IData
    {
        byte[] GetData(Vector3<double> lonLatZoom);

        uint GetTileSize();
    }
}
