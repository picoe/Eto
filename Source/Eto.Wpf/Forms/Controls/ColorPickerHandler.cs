using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xwt = Xceed.Wpf.Toolkit;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Wpf.Forms.Controls
{
	public class ColorPickerHandler : WpfControl<xwt.ColorPicker, ColorPicker, ColorPicker.ICallback>, ColorPicker.IHandler
	{
		public ColorPickerHandler()
		{
			Control = new xwt.ColorPicker { Focusable = true, IsTabStop = true };
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ColorPicker.ColorChangedEvent:
					Control.SelectedColorChanged += (sender, e) => Callback.OnColorChanged(Widget, EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public Eto.Drawing.Color Color
		{
			get { return Control.SelectedColor.ToEto(); }
			set { Control.SelectedColor = value.ToWpf(); }
		}
	}
}
