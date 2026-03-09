namespace Eto.WinForms;

static class DpiExtensions
{
	public static Size DeviceUnitsToLogical(this swf.Control control, sd.Size size)
	{
#if NET9_0_OR_GREATER
		var dpi = control.DeviceDpi;
		return new Size((int)Math.Ceiling(size.Width * 96.0 / dpi), (int)Math.Ceiling(size.Height * 96.0 / dpi));
#else
		return size.ToEto();
#endif
	}

	public static Rectangle DeviceUnitsToLogical(this swf.Control control, sd.Rectangle rectangle)
	{
#if NET9_0_OR_GREATER
		var location = control.DeviceUnitsToLogical(rectangle.Location);
		var size = control.DeviceUnitsToLogical(rectangle.Size);
		return new Rectangle(location, size);
#else
		return rectangle.ToEto();
#endif
	}
	public static Point DeviceUnitsToLogical(this swf.Control control, sd.Point point)
	{
#if NET9_0_OR_GREATER
		var dpi = control?.DeviceDpi ?? 96;
		return new Point((int)Math.Ceiling(point.X * 96.0 / dpi), (int)Math.Ceiling(point.Y * 96.0 / dpi));
#else
		return point.ToEto();
#endif
	}
	public static int DeviceUnitsToLogical(this swf.Control control, int size)
	{
#if NET9_0_OR_GREATER
		var dpi = control?.DeviceDpi ?? 96;
		return (int)Math.Ceiling(size * 96.0 / dpi);
#else
		return size;
#endif
	}

	public static sd.Size LogicalToDeviceUnits(this swf.Control control, Size size)
	{
#if NET9_0_OR_GREATER
		var dpi = control?.DeviceDpi ?? 96;
		return new sd.Size((int)Math.Ceiling(size.Width * dpi / 96.0), (int)Math.Ceiling(size.Height * dpi / 96.0));
#else
		return size.ToSD();
#endif
	}

	public static sd.Point LogicalToDeviceUnits(this swf.Control control, Point point)
	{
#if NET9_0_OR_GREATER
		var dpi = control?.DeviceDpi ?? 96;
		return new sd.Point((int)Math.Ceiling(point.X * dpi / 96.0), (int)Math.Ceiling(point.Y * dpi / 96.0));		
#else
		return point.ToSD();
#endif
	}

#if !NET9_0_OR_GREATER
	public static sd.Size LogicalToDeviceUnits(this swf.Control control, sd.Size size)
	{
		return size;
	}
	public static int LogicalToDeviceUnits(this swf.Control control, int size)
	{
		return size;
	}
#endif

}
