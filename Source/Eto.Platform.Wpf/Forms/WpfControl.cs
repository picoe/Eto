using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms
{
	public class WpfControl<T, W> : WpfFrameworkElement<T, W>, IControl
		where T : System.Windows.Controls.Control
		where W: Control
	{
		public override Color BackgroundColor
		{
			get
			{
				var brush = Control.Background as System.Windows.Media.SolidColorBrush;
				if (brush != null) return Generator.Convert(brush.Color);
				else return Color.Black;
			}
			set
			{
				Control.Background = new System.Windows.Media.SolidColorBrush(Generator.Convert(value));
			}
		}
	}
}
