using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp
{
	public class MouseHandler : IMouse
	{
		public void Initialize ()
		{
		}

		public Widget Widget { get; set; }

		public Eto.Platform Platform { get; set; }

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

