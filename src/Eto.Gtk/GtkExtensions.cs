using System;
using Eto.Drawing;

namespace Eto.GtkSharp
{
	static class GtkExtensions
	{
		public static void RemoveAllChildren(this Gtk.Container widget)
		{
			foreach (Gtk.Widget w in widget.Children)
			{
				widget.Remove(w);
			}
		}

		public static Point GetScreenPosition(this Gtk.Widget widget)
		{
			var parent = widget.ParentWindow;
			widget.GetWindow().GetOrigin(out var x, out var y);

			if (parent != null)
			{
				parent.GetOrigin(out var wx, out var wy);
				var extents = parent.FrameExtents;
				//x += (wx + extents.X) / parent.ScaleFactor;
				//y += (wy + extents.Y) / parent.ScaleFactor;
			}

			return new Point(x, y);
		}
	}
}
