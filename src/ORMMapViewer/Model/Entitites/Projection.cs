using ORMMap.VectorTile.Geometry;

namespace ORMMapViewer.Model.Entitites
{
	public struct Projection
	{
		public double Top;
		public double Left;
		public double Right;
		public double Bottom;

		public Projection(double top, double left, double right, double bottom)
		{
			this.Top = top;
			this.Left = left;
			this.Right = right;
			this.Bottom = bottom;
		}

		public Projection(double topBottom, double leftRight)
		{
			this.Top = this.Bottom = topBottom;
			this.Left = this.Right = leftRight;
		}
	}
}
