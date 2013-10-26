
namespace Eto.Platform.GtkSharp
{
#if GTK2

	public static class Gtk3Compatibility
	{
		public static void SetAllocation(this Gtk.Widget widget, Gdk.Rectangle rectangle)
		{
			widget.Allocation = rectangle;
		}
	}

	public interface ITreeModelImplementor : Gtk.TreeModelImplementor
	{
	}

#else

	public interface ITreeModelImplementor : Gtk.ITreeModelImplementor
	{
	}

#endif
}

