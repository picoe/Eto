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

		public GroupBoxHandler()
		{
			Control = new swc.GroupBox();
		}

		public override void SetContainerContent(sw.FrameworkElement content)
		{
			Control.Content = content;
			var margin = content.Margin;
			margin.Bottom = 10;
			content.Margin = margin;
		}

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}

		public Font Font
		{
			get { return font; }
			set
			{
				font = value;
				FontHandler.Apply(Control, font);
			}
		}

		public string Text
		{
			get { return Control.Header as string; }
			set { Control.Header = value; }
		}
	}
}
