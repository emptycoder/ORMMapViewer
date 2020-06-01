using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ORMMap.Model.Entitites
{
	public struct Pallete : IDisposable
	{
		private static SolidBrush solidBrush = new SolidBrush(Color.Black);

		private readonly Color mainFillColor;
		private readonly Pen mainDrawPen;

		private Dictionary<string, Color> propsFillColors;
		private Dictionary<string, Pen> propsDrawPens;

		public Pallete(Color mainFillColor, Color mainDrawColor, float thickness = 1)
		{
			this.mainFillColor = mainFillColor;
			mainDrawPen = new Pen(mainDrawColor, thickness);

			propsFillColors = null;
			propsDrawPens = null;
		}

		public Pen GetMainDrawPen()
		{
			return mainDrawPen;
		}

		public SolidBrush GetMainFillBrush()
		{
			solidBrush.Color = mainFillColor;
			return solidBrush;
		}

		public Pallete AddPropFillBrush(string prop, Color color)
		{
			if (propsFillColors == null)
			{
				propsFillColors = new Dictionary<string, Color> {{prop, color}};
			}
			else
			{
				propsFillColors.Add(prop, color);
			}

			return this;
		}

		public Pallete AddPropDrawPen(string prop, Color color, float thickness = 1)
		{
			if (propsDrawPens == null)
			{
				propsDrawPens = new Dictionary<string, Pen> {{prop, new Pen(color, thickness)}};
			}
			else
			{
				propsDrawPens.Add(prop, new Pen(color, thickness));
			}

			return this;
		}

		public Pen GetPropDrawPen(string prop)
		{
			if (propsDrawPens != null && propsDrawPens.TryGetValue(prop, out Pen pen))
			{
				return pen;
			}

			return mainDrawPen;
		}

		public SolidBrush GetPropFillBrush(string prop)
		{
			if (propsFillColors != null && propsFillColors.TryGetValue(prop, out Color color))
			{
				solidBrush.Color = color;
				return solidBrush;
			}

			solidBrush.Color = mainFillColor;
			return solidBrush;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder =
				new StringBuilder(
					$"Pallete:\n Main fill color: {mainFillColor}\n Main draw color: {mainDrawPen.Color}\n Draw prop colors:");
			foreach (KeyValuePair<string, Pen> pair in propsDrawPens)
			{
				stringBuilder.Append($"  {pair.Key}: {pair.Value.Color}\n");
			}

			stringBuilder.Append("\n Fill prop colors:");
			foreach (KeyValuePair<string, Color> pair in propsFillColors)
			{
				stringBuilder.Append($"  {pair.Key}: {pair.Value}\n");
			}

			return stringBuilder.ToString();
		}

		public void Dispose()
		{
			mainDrawPen.Dispose();

			foreach (KeyValuePair<string, Pen> pair in propsDrawPens)
			{
				pair.Value.Dispose();
			}
		}
	}
}
