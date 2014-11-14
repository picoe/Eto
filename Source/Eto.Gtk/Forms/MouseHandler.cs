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
				Gdk.Display.Default.GetPointer (out x, out y);
				return new PointF (x, y);
			}
		}

		public MouseButtons Buttons
		{
			get
			{
				int x, y;
				Gdk.ModifierType modifier;
				Gdk.Display.Default.GetPointer (out x, out y, out modifier);
				return modifier.ToEtoMouseButtons ();
			}
		}
	}
}

