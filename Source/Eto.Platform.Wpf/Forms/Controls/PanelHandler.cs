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
				if (brush != null) return Generator.Convert (brush.Color);
				else return Color.Black;
			}
			set
			{
				Control.Background = new System.Windows.Media.SolidColorBrush (Generator.Convert (value));
			}
		}

	}
}
