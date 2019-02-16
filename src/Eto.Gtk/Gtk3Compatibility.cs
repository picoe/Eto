using Eto.Drawing;
using System;

namespace Eto.GtkSharp
{
#if GTK2
	public struct RGBA
	{

	}
#endif

	public static class Gtk3Compatibility
	{
#if GTK2
		public static void SetSourceColor(this Cairo.Context context, Cairo.Color color)
		{
			context.SetSourceRGBA(color.R, color.G, color.B, color.A);
		}

		public static void SetAllocation(this Gtk.Widget widget, Gdk.Rectangle rectangle)
		{
			widget.Allocation = rectangle;
		}
		
		public static Gtk.StateType ToGtk(this GtkStateFlags flags)
		{
			switch (flags)
			{
				case GtkStateFlags.Normal:
					return Gtk.StateType.Normal;
				case GtkStateFlags.Active:
					return Gtk.StateType.Active;
				case GtkStateFlags.Prelight:
					return Gtk.StateType.Prelight;
				case GtkStateFlags.Insensitive:
					return Gtk.StateType.Insensitive;
				case GtkStateFlags.Selected:
					return Gtk.StateType.Selected;
				default:
					throw new NotSupportedException();
			}
		}

		public static Gdk.Window GetWindow(this Gtk.Widget widget)
		{
			if (widget is Gtk.Button b)
			{
				var eventWindowPtr = NativeMethods.gtk_button_get_event_window(b.Handle);
				if (eventWindowPtr != IntPtr.Zero)
				{
					var window = GLib.Object.GetObject(eventWindowPtr) as Gdk.Window;
					if (window != null)
						return window;
				}
			}
			return widget.GdkWindow;
		}

		public static Gtk.Requisition GetPreferredSize(this Gtk.Widget widget)
		{
			return widget.SizeRequest();
		}

		public static Pango.FontDescription GetFont(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.Style.FontDescription;
		}

		public static void SetFont(this Gtk.Widget widget, Pango.FontDescription font)
		{
			widget.ModifyFont(font);
		}

		public static Color GetBackground(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.Style.Background(state.ToGtk()).ToEto();
		}

		public static void SetBackground(this Gtk.Widget widget, Color color, GtkStateFlags state = GtkStateFlags.Normal)
		{
			widget.ModifyBg(state.ToGtk(), color.ToGdk());
		}

		public static void ClearBackground(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			widget.ModifyBg(state.ToGtk());
		}

		public static Color GetBase(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.Style.Base(state.ToGtk()).ToEto();
		}

		public static void SetBase(this Gtk.Widget widget, Color color, GtkStateFlags state = GtkStateFlags.Normal)
		{
			widget.ModifyBase(state.ToGtk(), color.ToGdk());
		}

		public static Color GetTextColor(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.Style.Text(state.ToGtk()).ToEto();
		}

		public static void SetTextColor(this Gtk.Widget widget, Color color, GtkStateFlags state = GtkStateFlags.Normal)
		{
			widget.ModifyText(state.ToGtk(), color.ToGdk());
		}

		public static Color GetForeground(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.Style.Foreground(state.ToGtk()).ToEto();
		}

		public static void SetForeground(this Gtk.Widget widget, Color color, GtkStateFlags state = GtkStateFlags.Normal)
		{
			widget.ModifyFg(state.ToGtk(), color.ToGdk());
		}

		public static Gdk.Atom[] ListTargets(this Gtk.SelectionData data) => data.Targets;

		public static Gdk.Atom[] ListTargets(this Gdk.DragContext context) => context.Targets;

		public static Gdk.Atom GetDataType(this Gtk.SelectionData data) => data.Type;
#else
		public static Gtk.StateFlags ToGtk(this GtkStateFlags flags)
		{
			switch (flags)
			{
				case GtkStateFlags.Normal:
					return Gtk.StateFlags.Normal;
				case GtkStateFlags.Active:
					return Gtk.StateFlags.Active;
				case GtkStateFlags.Insensitive:
					return Gtk.StateFlags.Insensitive;
				case GtkStateFlags.Prelight:
					return Gtk.StateFlags.Prelight;
				case GtkStateFlags.Selected:
					return Gtk.StateFlags.Selected;
				default:
					throw new NotSupportedException();
			}
		}

		public static Gdk.Window GetWindow(this Gtk.Widget widget)
		{
			var window = (widget as Gtk.Button)?.EventWindow;
			return window ?? widget.Window;
		}

		public static Gtk.Requisition GetPreferredSize(this Gtk.Widget widget)
		{
			Gtk.Requisition minimum_size, natural_size;
			widget.GetPreferredSize(out minimum_size, out natural_size);
			return natural_size;
		}

		public static Cairo.Color ToCairo(this Gdk.RGBA color)
		{
			return new Cairo.Color(color.Red, color.Green, color.Blue, color.Alpha);
		}

		public static Color ToEto(this Gdk.RGBA color)
		{
			return new Color((float)color.Red, (float)color.Green, (float)color.Blue, (float)color.Alpha);
		}

		public static Gdk.RGBA ToRGBA(this Color color)
		{
			return new Gdk.RGBA { Red = color.R, Green = color.G, Blue = color.B, Alpha = color.A };
		}

		public static Pango.FontDescription GetFont(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.StyleContext.GetFont(state.ToGtk());
		}

		public static void SetFont(this Gtk.Widget widget, Pango.FontDescription font)
		{
			widget.OverrideFont(font);
		}

		public static Color GetBackground(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.StyleContext.GetBackgroundColor(state.ToGtk()).ToEto();
		}

		public static void SetBackground(this Gtk.Widget widget, Color color, GtkStateFlags state = GtkStateFlags.Normal)
		{
			widget.OverrideBackgroundColor(state.ToGtk(), color.ToRGBA());
		}

		public static void ClearBackground(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			// no way to do this using gtk-sharp 3?
			//widget.OverrideBackgroundColor(state, null);
		}

		public static Color GetBase(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.StyleContext.GetBackgroundColor(state.ToGtk()).ToEto();
		}

		public static void SetBase(this Gtk.Widget widget, Color color, GtkStateFlags state = GtkStateFlags.Normal)
		{
			widget.OverrideBackgroundColor(state.ToGtk(), color.ToRGBA());
		}

		public static Color GetTextColor(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.StyleContext.GetColor(state.ToGtk()).ToEto();
		}

		public static void SetTextColor(this Gtk.Widget widget, Color color, GtkStateFlags state = GtkStateFlags.Normal)
		{
			widget.OverrideColor(state.ToGtk(), color.ToRGBA());
		}

		public static Color GetForeground(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.StyleContext.GetColor(state.ToGtk()).ToEto();
		}

		public static void SetForeground(this Gtk.Widget widget, Color color, GtkStateFlags state = GtkStateFlags.Normal)
		{
			widget.OverrideColor(state.ToGtk(), color.ToRGBA());
		}

		public static Gdk.Atom GetDataType(this Gtk.SelectionData data) => data.DataType;

#if !GTKCORE
		public static Gtk.Widget GetChildAt(this Gtk.Grid grid, int left, int top)
		{
			var ptr = NativeMethods.gtk_grid_get_child_at(grid.Handle, left, top);
			return GLib.Object.GetObject(ptr) as Gtk.Widget;
		}
#endif
#endif

	}

#if GTK2
	public interface ITreeModelImplementor : Gtk.TreeModelImplementor
	{
	}

#else

	public interface ITreeModelImplementor : Gtk.ITreeModelImplementor
	{
	}

#endif
}

