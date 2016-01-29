using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using xwt = Xceed.Wpf.Toolkit;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using Eto.Forms;
using System.ComponentModel;

namespace Eto.Wpf.Forms.Controls
{
	public class ColorPickerHandler : WpfControl<xwt.ColorPicker, ColorPicker, ColorPicker.ICallback>, ColorPicker.IHandler
	{
		static DependencyPropertyDescriptor dpdIsOpen = DependencyPropertyDescriptor.FromProperty(xwt.ColorPicker.IsOpenProperty, typeof(xwt.ColorPicker));
		protected override Size DefaultSize { get { return new Size(60, -1); } }

		protected override bool PreventUserResize { get { return true; } }

		public ColorPickerHandler()
		{
			Control = new xwt.ColorPicker
			{
				Focusable = true,
				IsTabStop = true,
				UsingAlphaChannel = false,
				AvailableColorsSortingMode = xwt.ColorSortingMode.HueSaturationBrightness
			};
			foreach (var col in Control.StandardColors.Where(r => r.Color == swm.Colors.Transparent).ToList())
			{
				Control.StandardColors.Remove(col);
			}
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
			get
			{
				var col = Control.SelectedColor ?? swm.Colors.Transparent;
				return col.ToEto();
			}
			set { Control.SelectedColor = value.ToWpf(); }
		}
	}
}
