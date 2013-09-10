using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;
using sw = System.Windows;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class PanelHandler : WpfContainer<swc.Border, Panel>, IPanel
	{
		public PanelHandler ()
		{
			Control = new swc.Border ();
			Control.Background = swm.Brushes.Transparent; // so we get mouse events
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
			Control.Child = (sw.UIElement)layout.ControlObject;
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
				Control.Background = new swm.SolidColorBrush (value.ToWpf ());
			}
		}

		public override Size MinimumSize
		{
			get
			{
				return new Size ((int)Control.MinWidth, (int)Control.MinHeight);
			}
			set
			{
				Control.MinWidth = value.Value.Width;
				Control.MinHeight = value.Value.Height;
			}
		}
	}
}
