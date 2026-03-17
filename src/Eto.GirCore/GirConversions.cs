using System;

namespace Eto.GirCore;

static class GirConversions
{
	public static Size GetAllocation(this Gtk.Widget widget)
	{
		return new Size(widget.GetAllocatedWidth(), widget.GetAllocatedHeight());
	}

	public static Gtk.Justification ToGtk(this TextAlignment alignment)
	{
		return alignment switch
		{
			TextAlignment.Left => Gtk.Justification.Left,
			TextAlignment.Center => Gtk.Justification.Center,
			TextAlignment.Right => Gtk.Justification.Right,
			_ => throw new NotSupportedException(),
		};
	}

	public static float ToGtkAlign(this TextAlignment alignment)
	{
		return alignment switch
		{
			TextAlignment.Left => 0f,
			TextAlignment.Center => 0.5f,
			TextAlignment.Right => 1f,
			_ => throw new NotSupportedException(),
		};
	}
	
	public static float ToGtkAlign(this VerticalAlignment alignment)
	{
		return alignment switch
		{
			VerticalAlignment.Top => 0f,
			VerticalAlignment.Center => 0.5f,
			VerticalAlignment.Bottom => 1f,
			_ => throw new NotSupportedException(),
		};
	}
	
}
