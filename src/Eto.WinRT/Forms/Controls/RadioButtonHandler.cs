using System;
using swc = Windows.UI.Xaml.Controls;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	/// <summary>
	/// Radio button handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class RadioButtonHandler : WpfControl<swc.RadioButton, RadioButton, RadioButton.ICallback>, RadioButton.IHandler
	{
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
			get { return (Control.Content as string).ToEtoMneumonic(); }
			set { Control.Content = value.ToWpfMneumonic(); }
		}

		public Color TextColor
		{
			get { return Control.Foreground.ToEtoColor(); }
			set { Control.Foreground = value.ToWpfBrush(Control.Foreground); }
		}
	}
}
