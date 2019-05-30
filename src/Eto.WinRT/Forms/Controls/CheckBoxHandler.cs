using System;
using sw = Windows.UI.Xaml;
using swc = Windows.UI.Xaml.Controls;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	/// <summary>
	/// Checkbox handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class CheckBoxHandler : WpfControl<swc.CheckBox, CheckBox, CheckBox.ICallback>, CheckBox.IHandler
	{
		readonly swc.Border border;

		public override sw.FrameworkElement ContainerControl
		{
			get { return border; }
		}

		public CheckBoxHandler ()
		{
			Control = new swc.CheckBox {
				IsThreeState = false,
				VerticalAlignment = sw.VerticalAlignment.Center
			};
			Control.Checked += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
			Control.Unchecked += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
			Control.Indeterminate += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
			border = new swc.Border { Child = Control };
		}

		public override Eto.Drawing.Color BackgroundColor
		{
			get { return border.Background.ToEtoColor(); }
			set { border.Background = value.ToWpfBrush(Control.Background); }
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public bool? Checked
		{
			get { return Control.IsChecked; }
			set { Control.IsChecked = value; }
		}

		public string Text
		{
			get { return (Control.Content as string).ToEtoMneumonic(); }
			set { Control.Content = value.ToWpfMneumonic(); }
		}

		public bool ThreeState
		{
			get { return Control.IsThreeState; }
			set { Control.IsThreeState = value; }
		}

		public Color TextColor
		{
			get { return Control.Foreground.ToEtoColor(); }
			set { Control.Foreground = value.ToWpfBrush(Control.Foreground); }
		}
	}
}
