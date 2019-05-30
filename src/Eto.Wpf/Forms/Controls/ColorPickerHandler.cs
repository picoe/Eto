using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using xwt = Xceed.Wpf.Toolkit;
using sw = System.Windows;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using Eto.Forms;
using System.ComponentModel;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoColorPicker : xwt.ColorPicker, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}

	public class ColorPickerHandler : WpfControl<xwt.ColorPicker, ColorPicker, ColorPicker.ICallback>, ColorPicker.IHandler
	{
		protected override sw.Size DefaultSize => new sw.Size(60, double.NaN);

		protected override bool PreventUserResize { get { return true; } }

		public ColorPickerHandler()
		{
			Control = new EtoColorPicker
			{
				Handler = this,
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

		public bool AllowAlpha
		{
			get { return Control.UsingAlphaChannel; }
			set { Control.UsingAlphaChannel = value; }
		}

		public bool SupportsAllowAlpha => true;
	}
}
