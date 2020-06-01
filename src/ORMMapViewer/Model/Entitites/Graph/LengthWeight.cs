using System;

namespace ORMMapViewer.Model.Entitites
{
	internal class LengthWeight : Weight
	{
		private readonly float length;

		public LengthWeight(float length)
		{
			this.length = length;
		}

		public LengthWeight(Node first, Node second)
		{
			length = (float) Math.Sqrt(Math.Pow(first.pos.X - second.pos.X, 2) + Math.Pow(first.pos.Y - second.pos.Y, 2));
		}

		public override float Calculate()
		{
			return length;
		}
	}
}
