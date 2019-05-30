using Eto.Forms;
using Eto.Drawing;
using sw = Windows.UI.Xaml;
using swc = Windows.UI.Xaml.Controls;
using swm = Windows.UI.Xaml.Media;

namespace Eto.WinRT.Forms.Controls
{
	/// <summary>
	/// Panel handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PanelHandler : WpfPanel<swc.Border, Panel, Panel.ICallback>, Panel.IHandler
	{
		public PanelHandler ()
		{
			Control = new swc.Border
			{
#if TODO_XAML
				Focusable = false,
				Background = swm.Brushes.Transparent // to get mouse events
#endif
			};
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
