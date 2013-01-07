using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Wpf.Drawing;
using Eto.Cache;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class GroupBoxHandler : WpfContainer<swc.GroupBox, GroupBox>, IGroupBox
	{
		Font font;
		
		public GroupBoxHandler ()
		{
			Control = new swc.GroupBox ();
		}
		
		public override Size ClientSize
		{
			get { return this.Size; }
			set
			{
				// TODO
				this.Size = value;
			}
		}

		public override object ContainerObject
		{
			get { return Control; }
		}

		public override void SetLayout (Layout layout)
		{
			Control.Content = (System.Windows.UIElement)layout.ControlObject;
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

		public Font Font
		{
			get { return font; }
			set
			{
				font = value;
				FontHandler.Apply (Control, font);
			}
		}

		public override Size? MinimumSize
		{
			get
			{
				if (Control.MinHeight == 0 && Control.MinWidth == 0)
					return null;
				return new Eto.Drawing.Size ((int)Control.MinHeight, (int)Control.MinWidth);
			}
			set
			{
				if (value != null) {
					Control.MinHeight = value.Value.Height;
					Control.MinWidth = value.Value.Width;
				}
				else
					Control.MinWidth = Control.MinHeight = 0;
			}
		}

		public string Text
		{
			get { return Control.Header as string; }
			set { Control.Header = value; }
		}
	}
}
