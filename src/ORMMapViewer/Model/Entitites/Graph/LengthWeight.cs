using Newtonsoft.Json;
using System;

namespace ORMMapViewer.Model.Entitites
{
	public class LengthWeight : IWeight
	{
		public float Length { get; set; }

		[JsonConstructor]
		public LengthWeight(float length)
		{
			this.Length = length;
		}

		public LengthWeight(Node first, Node second)
		{
			Length = (float) Math.Sqrt(Math.Pow(first.pos.X - second.pos.X, 2) + Math.Pow(first.pos.Y - second.pos.Y, 2));
		}
	}
}
