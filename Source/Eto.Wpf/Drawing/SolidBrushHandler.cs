using Eto.Drawing;
using swm = System.Windows.Media;

namespace Eto.Wpf.Drawing
{
	/// <summary>
	/// Handler for <see cref="ISolidBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class SolidBrushHandler : SolidBrush.IHandler
	{
		public Color GetColor (SolidBrush widget)
		{
			return ((swm.SolidColorBrush)widget.ControlObject).Color.ToEto ();
		}

		public void SetColor (SolidBrush widget, Color color)
		{
			((swm.SolidColorBrush)widget.ControlObject).Color = color.ToWpf ();
		}

		public object Create (Color color)
		{
			return new swm.SolidColorBrush (color.ToWpf ());
		}
	}
}
