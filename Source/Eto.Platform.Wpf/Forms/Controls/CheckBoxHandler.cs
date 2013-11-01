using System;
using sw = System.Windows;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class CheckBoxHandler : WpfControl<swc.CheckBox, CheckBox>, ICheckBox
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
			Control.Checked += delegate {
				Widget.OnCheckedChanged (EventArgs.Empty);
			};
			Control.Unchecked += delegate {
				Widget.OnCheckedChanged (EventArgs.Empty);
			};
			border = new swc.Border { Child = Control };
		}

		public override Eto.Drawing.Color BackgroundColor
		{
			get { return border.Background.ToEtoColor(); }
			set { border.Background = value.ToWpfBrush(Control.Background); }
		}

		public override bool UseMousePreview { get { return true; } }

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
	}
}
