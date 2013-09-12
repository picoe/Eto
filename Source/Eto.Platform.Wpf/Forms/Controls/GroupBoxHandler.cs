using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Wpf.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class GroupBoxHandler : WpfDockContainer<swc.GroupBox, GroupBox>, IGroupBox
	{
		Font font;
		
		public GroupBoxHandler ()
		{
			Control = new swc.GroupBox();
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

		public override void SetContainerContent(sw.FrameworkElement content)
		{
			Control.Content = content;
			/*var tableLayout = layout.Handler as TableLayoutHandler;
			if (tableLayout != null)
				tableLayout.Adjust = new Size(0, -1);*/
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

		public Font Font
		{
			get { return font; }
			set
			{
				font = value;
				FontHandler.Apply (Control, font);
			}
		}

		public string Text
		{
			get { return Control.Header as string; }
			set { Control.Header = value; }
		}
	}
}
