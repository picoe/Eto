using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;
using Eto.Cache;
using swm = System.Windows.Media;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class PanelHandler : WpfContainer<System.Windows.Controls.Border, Panel>, IPanel
	{
		public PanelHandler ()
		{
			Control = new System.Windows.Controls.Border ();
			//Control.Background = System.Windows.SystemColors.ControlBrush;
		}

		public override Size ClientSize
		{
			get { return this.Size; }
			set { this.Size = value; }
		}

		public override object ContainerObject
		{
			get { return Control; }
		}

		public override void SetLayout (Layout layout)
		{
			Control.Child = (System.Windows.UIElement)layout.ControlObject;
		}

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
				Control.Background = Brushes.Cached(value, this.Generator).ControlObject as swm.Brush;
			}
		}

		public override Size? MinimumSize
		{
			get
			{
				if (Control.MinWidth > 0 && Control.MinHeight > 0)
					return new Size ((int)Control.MinWidth, (int)Control.MinHeight);
				else
					return null;
			}
			set
			{
				if (value != null) {
					Control.MinWidth = value.Value.Width;
					Control.MinHeight = value.Value.Height;
				}
				else {
					Control.MinHeight = 0;
					Control.MinWidth = 0;
				}
			}
		}
	}
}
