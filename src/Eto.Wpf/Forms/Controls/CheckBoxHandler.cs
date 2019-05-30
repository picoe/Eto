using System;
using sw = System.Windows;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Wpf.Forms.Controls
{
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
				VerticalAlignment = sw.VerticalAlignment.Center,
				VerticalContentAlignment = sw.VerticalAlignment.Center
			};
			Control.Checked += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
			Control.Unchecked += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
			Control.Indeterminate += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);

			border = new EtoBorder { Handler = this, Child = Control };
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
			get { return (Control.Content as string).ToEtoMnemonic(); }
			set { Control.Content = value.ToPlatformMnemonic(); }
		}

		public bool ThreeState
		{
			get { return Control.IsThreeState; }
			set { Control.IsThreeState = value; }
		}
	}
}
