using System;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class RadioButtonHandler : WpfControl<swc.RadioButton, RadioButton, RadioButton.ICallback>, RadioButton.IHandler
	{
		swc.Border border;

		public override sw.FrameworkElement ContainerControl
		{
			get { return border; }
		}

		public void Create(RadioButton controller)
		{
			Control = new swc.RadioButton
			{
				VerticalAlignment = sw.VerticalAlignment.Center,
				VerticalContentAlignment = sw.VerticalAlignment.Center,
			};
			if (controller != null)
			{
				var parent = (swc.RadioButton)controller.ControlObject;
				Control.GroupName = parent.GroupName;
			}
			else
				Control.GroupName = Guid.NewGuid().ToString();

			Control.Loaded += Control_Loaded;
			Control.Checked += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
			Control.Unchecked += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);

			border = new EtoBorder { Handler = this, Child = Control };
		}

		void Control_Loaded(object sender, sw.RoutedEventArgs e)
		{
			var border = Control.FindChild<swc.Border>("radioButtonBorder");
			if (border != null)
			{
				// ensure the radio button and dot is always round and in the center regardless of DPI
				border.UseLayoutRounding = false;
				border.SnapsToDevicePixels = false;
			}
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public bool Checked
		{
			get { return Control.IsChecked ?? false; }
			set { Control.IsChecked = value; }
		}

		public string Text
		{
			get { return (Control.Content as string).ToEtoMnemonic(); }
			set { Control.Content = value.ToPlatformMnemonic(); }
		}

		public override Color BackgroundColor
		{
			get { return border.Background.ToEtoColor(); }
			set { border.Background = value.ToWpfBrush(border.Background); }
		}
	}
}
