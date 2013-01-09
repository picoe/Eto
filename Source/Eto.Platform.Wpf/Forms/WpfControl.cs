using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Wpf.Drawing;
using swm = System.Windows.Media;

namespace Eto.Platform.Wpf.Forms
{
	public class WpfControl<T, W> : WpfFrameworkElement<T, W>, IControl
		where T : System.Windows.Controls.Control
		where W: Control
	{
		Font font;

		public override Color BackgroundColor
		{
			get
			{
				var brush = Control.Background as swm.SolidColorBrush;
                if (brush != null) return brush.Color.ToEto ();
                else return Colors.Black;
			}
			set
			{
				Control.Background = new swm.SolidColorBrush (value.ToWpf ());
            }
		}

		public Font Font
		{
			get
			{
				if (font == null)
					font = new Font (Widget.Generator, new FontHandler (Widget.Generator, Control));
				return font;
			}
			set
			{
				font = value;
				FontHandler.Apply (Control, font);
			}
		}

	}
}
