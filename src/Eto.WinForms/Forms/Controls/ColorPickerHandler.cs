using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swf = System.Windows.Forms;
using sd = System.Drawing;

namespace Eto.WinForms.Forms.Controls
{
	public class ColorPickerHandler : WindowsControl<swf.Button, ColorPicker, ColorPicker.ICallback>, ColorPicker.IHandler
	{
		public ColorPickerHandler()
		{
			Control = new swf.Button { Width = 40 };
			Control.Click += HandleClick;
			Control.BackColor = sd.Color.Black;
		}

		public override Color BackgroundColor
		{
			get { return Colors.Transparent; }
			set
			{
				// cannot set background color, we use that as the currently selected color
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ColorPicker.ColorChangedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void HandleClick(object sender, EventArgs e)
		{
			using (Widget.Platform.Context)
			{
				var picker = new ColorDialog { Color = Color, AllowAlpha = AllowAlpha };
				var result = picker.ShowDialog(Widget);
				if (result == DialogResult.Ok)
				{
					Color = picker.Color;
				}
			}
		}

		public Eto.Drawing.Color Color
		{
			get { return Control.BackColor.ToEto(); }
			set
			{
				var color = value.ToSD();
				if (Control.BackColor != color)
				{
					Control.BackColor = color;
					Callback.OnColorChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public bool AllowAlpha { get; set; }

		public bool SupportsAllowAlpha => false;

		static readonly Win32.WM[] intrinsicEvents = { Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}
	}
}
