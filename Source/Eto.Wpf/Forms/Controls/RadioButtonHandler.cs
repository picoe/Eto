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
			Control = new swc.RadioButton();
			if (controller != null)
			{
				var parent = (swc.RadioButton)controller.ControlObject;
				Control.GroupName = parent.GroupName;
			}
			else
				Control.GroupName = Guid.NewGuid().ToString();

			Control.Checked += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
			Control.Unchecked += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
			border = new swc.Border { Child = Control };
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
