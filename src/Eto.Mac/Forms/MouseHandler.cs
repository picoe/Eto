using System;
using Eto.Forms;
using System.Runtime.InteropServices;

using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;

#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#endif

namespace Eto.Mac.Forms
{
	public class MouseHandler : Mouse.IHandler
	{
		public void Initialize ()
		{
		}
		public Widget Widget { get; set; }

		public Eto.Platform Platform { get; set; }

		[DllImport(Constants.CoreGraphicsLibrary)]
 		static extern int CGWarpMouseCursorPosition(CGPoint point);

		public void SetCursor(Cursor cursor) => cursor.ToNS().Set();

		public Eto.Drawing.PointF Position
		{
			get
			{
				var mouseLocation = NSEvent.CurrentMouseLocation;
				var origin = NSScreen.Screens[0].Frame.Bottom;
				mouseLocation.Y = origin - mouseLocation.Y;
				return mouseLocation.ToEto();
			}
			set
			{
				var origin = NSScreen.Screens[0].Frame.Bottom;
				var mouseLocation = value.ToNS();
				//mouseLocation.Y = origin - mouseLocation.Y;
				CGWarpMouseCursorPosition(mouseLocation);
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

