using ORMMap.VectorTile.Geometry;
using System.Windows.Media.Media3D;

namespace ORMMapViewer.Model.Entitites
{
	public sealed class RaycastMapResult
	{
		public readonly LatLng TileLatLng;
		public readonly Point3D HitPoint;
		public readonly Vector2<double> PointLatLng;

		public RaycastMapResult(LatLng tileLatLng, Vector2<double> pointLatLng, Point3D hitPoint)
		{
			this.TileLatLng = tileLatLng;
			this.PointLatLng = pointLatLng;
			this.HitPoint = hitPoint;
		}

		public override string ToString()
		{
			return $"Raycast tileLatLng: {TileLatLng} on coordLatLng: {PointLatLng}.";
		}
	}
}
