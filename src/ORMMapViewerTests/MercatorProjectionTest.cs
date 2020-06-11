using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORMMap;
using ORMMap.VectorTile.Geometry;

namespace ORMMapViewerTests
{
	[TestClass]
	public class MercatorProjectionTest
	{
		[TestMethod]
		public void TestMercatorProjection()
		{
			LatLng startPosition = new LatLng(48.464717, 35.046183);

			Vector2<double> tile = MercatorProjection.LatLngToTile(startPosition, 13);
			LatLng latLng = MercatorProjection.TileToLatLng(tile, 13);
			LatLng latLng1 = MercatorProjection.TileToLatLng(tile.X, tile.Y, 13);

			Assert.IsTrue(startPosition.IsEquals(latLng.Round(6)) && startPosition.IsEquals(latLng1.Round(6)));
		}
	}
}
