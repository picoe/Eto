using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using xwt = Xceed.Wpf.Toolkit;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Wpf.Forms.Controls
{
	public class ColorPickerHandler : WpfControl<xwt.ColorPicker, ColorPicker, ColorPicker.ICallback>, ColorPicker.IHandler
	{
		protected override Size DefaultSize { get { return new Size(60, -1); } }

		protected override bool PreventUserResize { get { return true; } }

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
