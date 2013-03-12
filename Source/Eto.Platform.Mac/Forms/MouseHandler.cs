using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms
{
	public class MouseHandler : IMouse
	{
		public void Initialize ()
		{
		}
		public Widget Widget { get; set; }

		public Eto.Generator Generator { get; set; }

		public Eto.Drawing.PointF Position
		{
			get
			{
				var mouseLocation = NSEvent.CurrentMouseLocation;
				//mouseLocation = Control.ConvertPointFromView (mouseLocation, null);
				//mouseLocation.Y = Control.Frame.Height - mouseLocation.Y;
				return Platform.Conversions.ToEto (mouseLocation);
			}
		}

		public MouseButtons Buttons
		{
			get
			{
				var current = NSEvent.CurrentPressedMouseButtons;
				var buttons = MouseButtons.None;
				if ((current & 1) != 0)
					buttons |= MouseButtons.Primary;
				if ((current & 2) != 0)
					buttons |= MouseButtons.Alternate;
				if ((current & 4) != 0)
					buttons |= MouseButtons.Middle;
				return buttons;
			}
		}
	}
}

