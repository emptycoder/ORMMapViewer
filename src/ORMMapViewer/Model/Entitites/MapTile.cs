using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using ORMMap.VectorTile.Geometry;
using ORMMapViewer;

namespace ORMMap.Model.Entitites
{
	public class MapTile
	{
		public const int size = 4096;

		private static readonly DiffuseMaterial defaultMaterial = new DiffuseMaterial
		{
			Brush = new ImageBrush
			{
				ViewboxUnits = BrushMappingMode.Absolute,
				TileMode = TileMode.Tile,
				Viewport = new Rect(new Point(0, 0), new Point(size, size)),
				Stretch = Stretch.None,
				AlignmentX = AlignmentX.Left,
				AlignmentY = AlignmentY.Top
			}
		};

		private static readonly Int32Collection defaultIndices = new Int32Collection
		{
			0, 1, 3, 1, 2, 3
		};

		private static readonly PointCollection defaultTextureCoordinates = new PointCollection
		{
			new Point(0, 0),
			new Point(-4096, 0),
			new Point(-4096, 4096),
			new Point(0, 4096)
		};

		public Vector3<double> lonLatZoom;
		public Vector2<int> hostPos;

		public GeometryModel3D model = new GeometryModel3D();
		public Vector2<int> pos;

		public MapTile(Vector2<int> position)
		{
			pos = position;
			hostPos = MainWindow.getTileHostPosFromLocalPos(position);
			MeshGeometry3D geometry = new MeshGeometry3D
			{
				Positions = new Point3DCollection
				{
					new Point3D(pos.X * size, pos.Y * size, 0),
					new Point3D((pos.X + 1) * size, pos.Y * size, 0),
					new Point3D((pos.X + 1) * size, (pos.Y + 1) * size, 0),
					new Point3D(pos.X * size, (pos.Y + 1) * size, 0)
				},
				TriangleIndices = defaultIndices,
				TextureCoordinates = defaultTextureCoordinates
			};

			model.Geometry = geometry;
			model.Material = defaultMaterial.Clone();
		}

		public void SetImageSource(BitmapSource source)
		{
			((ImageBrush) ((DiffuseMaterial) model.Material).Brush).ImageSource = source;
		}
	}
}
