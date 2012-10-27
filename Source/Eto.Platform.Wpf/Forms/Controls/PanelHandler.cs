using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class PanelHandler : WpfFrameworkElement<System.Windows.Controls.Border, Panel>, IPanel
	{
		public PanelHandler ()
		{
			Control = new System.Windows.Controls.Border ();
			//Control.Background = System.Windows.SystemColors.ControlBrush;
		}

		public Size ClientSize
		{
			get { return this.Size; }
			set { this.Size = value; }
		}

		public object ContainerObject
		{
			get { return Control; }
		}

		public virtual void SetLayout (Layout layout)
		{
			Control.Child = (System.Windows.UIElement)layout.ControlObject;
		}

		public override Color BackgroundColor
		{
			get
			{
				var brush = Control.Background as System.Windows.Media.SolidColorBrush;
				if (brush != null) return brush.Color.ToEto ();
				else return Colors.Black;
			}
			set
			{
				Control.Background = new System.Windows.Media.SolidColorBrush (value.ToWpf ());
			}
		}

		public Size? MinimumSize
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
