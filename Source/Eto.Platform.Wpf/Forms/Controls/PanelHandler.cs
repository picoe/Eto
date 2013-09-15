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
	public class PanelHandler : WpfDockContainer<swc.Border, Panel>, IPanel
	{
		public PanelHandler ()
		{
			Control = new swc.Border ();
			Control.Background = swm.Brushes.Transparent; // so we get mouse events
		}

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}

		public override void SetContainerContent(sw.FrameworkElement content)
		{
			Control.Child = content;
		}
	}
}
