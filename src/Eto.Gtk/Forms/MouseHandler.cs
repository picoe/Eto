using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms
{
	public class MouseHandler : Mouse.IHandler
	{
		public PointF Position
		{
			get
			{
				int x, y;
				Gdk.Display.Default.GetPointer(out x, out y);
				return new PointF(x, y);
			}
			set
			{
				var position = value;
				var screen = Screen.FromPoint(position);
				var gdkScreen = ScreenHandler.GetControl(screen);
				// on mac, 0,0 is the primary screen even in gtk
				if (EtoEnvironment.Platform.IsMac)
				{
					position.Y -= Screen.PrimaryScreen.Bounds.Y;
				}
				else
				{
					// not sure if this is correct, I cannot find a way to test this properly using a parallels VM
					// with multiple displays.
					position -= screen.Bounds.TopLeft;
				}
#if GTK3
				Gdk.Display.Default.DeviceManager.ClientPointer.Warp(gdkScreen, (int)position.X, (int)position.Y);
#elif GTK2
				Gdk.Display.Default.WarpPointer(gdkScreen, (int)position.X, (int)position.Y);
#endif
			}
		}

		public MouseButtons Buttons
		{
			get
			{
				int x, y;
				Gdk.ModifierType modifier;
				Gdk.Display.Default.GetPointer(out x, out y, out modifier);
				return modifier.ToEtoMouseButtons();
			}
		}

		public void SetCursor(Cursor cursor)
		{
			
		}
	}
}