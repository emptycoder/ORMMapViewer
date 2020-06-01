using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ORMMap.Model.Entitites
{
	public struct Pallete
	{
		public readonly Color MainFillColor;
		public readonly Color MainDrawColor;
		public readonly float Thickness;

		private Dictionary<string, Color> propsFillColor;
		private Dictionary<string, Color> propsDrawColor;

		public Pallete(Color mainFillColor, Color mainDrawColor, float thickness)
		{
			MainFillColor = mainFillColor;
			MainDrawColor = mainDrawColor;
			Thickness = thickness;

			propsFillColor = propsDrawColor = null;
		}

		public Pallete AddPropFillColor(string prop, Color color)
		{
			if (propsFillColor == null)
				propsFillColor = new Dictionary<string, Color> {{prop, color}};
			else
				propsFillColor.Add(prop, color);

			return this;
		}

		public Pallete AddPropDrawColor(string prop, Color color)
		{
			if (propsDrawColor == null)
				propsDrawColor = new Dictionary<string, Color> {{prop, color}};
			else
				propsDrawColor.Add(prop, color);

			return this;
		}

		public Color GetPropDrawColor(string prop)
		{
			if (propsDrawColor != null && propsDrawColor.TryGetValue(prop, out var color)) return color;

			return MainDrawColor;
		}

		public Color GetPropFillColor(string prop)
		{
			if (propsFillColor != null && propsFillColor.TryGetValue(prop, out var color)) return color;

			return MainFillColor;
		}

		public override string ToString()
		{
			var stringBuilder =
				new StringBuilder(
					$"Pallete:\n Main fill color: {MainFillColor}\n Main draw color: {MainDrawColor}\n Draw prop colors:");
			foreach (var pair in propsDrawColor) stringBuilder.Append($"  {pair.Key}: {pair.Value}\n");
			stringBuilder.Append("\n Fill prop colors:");
			foreach (var pair in propsFillColor) stringBuilder.Append($"  {pair.Key}: {pair.Value}\n");
			return stringBuilder.ToString();
		}
	}
}
