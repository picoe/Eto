using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;

namespace Eto.Platform.Wpf.Drawing
{
	/// <summary>
	/// Handler for <see cref="ISolidBrush"/>
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class SolidBrushHandler : ISolidBrush
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
