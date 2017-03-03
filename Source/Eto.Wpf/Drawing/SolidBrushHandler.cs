using Eto.Drawing;
using swm = System.Windows.Media;

namespace Eto.Wpf.Drawing
{
	class FrozenBrushWrapper
	{
		public FrozenBrushWrapper(swm.Brush brush)
		{
			Brush = brush;
			SetFrozen();
		}

		public swm.Brush Brush { get; }

		public swm.Brush FrozenBrush { get; private set; }

		public void SetFrozen() => FrozenBrush = (swm.Brush)Brush.GetAsFrozen();
	}

	/// <summary>
	/// Handler for <see cref="ISolidBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class SolidBrushHandler : SolidBrush.IHandler
	{
		static swm.SolidColorBrush Get(SolidBrush widget) => ((FrozenBrushWrapper)widget.ControlObject).Brush as swm.SolidColorBrush;

		static void SetFrozen(SolidBrush widget) => ((FrozenBrushWrapper)widget.ControlObject).SetFrozen();

		public Color GetColor(SolidBrush widget)
		{
			return Get(widget).Color.ToEto();
		}

		public void SetColor(SolidBrush widget, Color color)
		{
			Get(widget).Color = color.ToWpf();
			SetFrozen(widget);
		}

		public object Create(Color color)
		{
			return new FrozenBrushWrapper(new swm.SolidColorBrush(color.ToWpf()));
		}
	}
}
