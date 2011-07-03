using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Wpf.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class LabelHandler : WpfControl<System.Windows.Controls.Label, Label>, ILabel
	{
		Font font;

		public LabelHandler ()
		{
			Control = new System.Windows.Controls.Label ();
		}

		public HorizontalAlign HorizontalAlign
		{
			get; set; 
		}

		public VerticalAlign VerticalAlign
		{
			get; set; 
		}

		public Font Font
		{
			get { return font; }
			set
			{
				font = value;
				if (font != null) {
					var handler = (FontHandler)font.Handler;
					handler.Apply (Control);
				}
			}
		}

		public WrapMode Wrap
		{
			get; set; 
		}

		public Color TextColor
		{
			get; set; 
		}

		public string Text
		{
			get { return Control.Content as string; }
			set { Control.Content = value; }
		}
	}
}
