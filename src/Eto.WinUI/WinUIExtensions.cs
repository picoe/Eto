using Eto.WinUI.Forms;
namespace Eto.WinUI;

static class WinUIExtensions
{
	public static double Horizontal(this mux.Thickness thickness)
	{
		return thickness.Left + thickness.Right;
	}

	public static double Vertical(this mux.Thickness thickness)
	{
		return thickness.Top + thickness.Bottom;
	}

	public static sw.Size Size(this mux.Thickness thickness)
	{
		return new sw.Size(thickness.Horizontal(), thickness.Vertical());
	}

	public static sw.Size Max(this sw.Size size1, sw.Size size2)
	{
		return new sw.Size(Math.Max(size1.Width, size2.Width), Math.Max(size1.Height, size2.Height));
	}

	public static sw.Size Min(this sw.Size size1, sw.Size size2)
	{
		return new sw.Size(Math.Min(size1.Width, size2.Width), Math.Min(size1.Height, size2.Height));
	}

	public static sw.Size Ceiling(this sw.Size size)
	{
		return new sw.Size(Math.Ceiling(size.Width), Math.Ceiling(size.Height));
	}

	public static sw.Size Floor(this sw.Size size)
	{
		return new sw.Size(Math.Floor(size.Width), Math.Floor(size.Height));
	}

	public static sw.Rect NormalizedRect(double x, double y, double width, double height)
	{
		if (width < 0)
		{
			x += width;
			width = -width + 1;
		}
		if (height < 0)
		{
			x += height;
			height = -height + 1;
		}
		return new sw.Rect(x, y, width, height);
	}

	public static sw.Size IfInfinity(this sw.Size size1, sw.Size size2)
	{
		if (double.IsInfinity(size1.Width))
			size1.Width = size2.Width;

		if (double.IsInfinity(size1.Height))
			size1.Height = size2.Height;
		return size1;
	}


	public static sw.Size IfNaN(this sw.Size size1, sw.Size size2)
	{
		if (double.IsNaN(size1.Width))
			size1.Width = size2.Width;

		if (double.IsNaN(size1.Height))
			size1.Height = size2.Height;
		return size1;
	}

	public static sw.Size ZeroIfNan(this sw.Size size)
	{
		if (double.IsNaN(size.Width))
			size.Width = 0;
		if (double.IsNaN(size.Height))
			size.Height = 0;
		return size;
	}

	public static sw.Size ZeroIfInfinity(this sw.Size size)
	{
		if (double.IsInfinity(size.Width))
			size.Width = 0;
		if (double.IsInfinity(size.Height))
			size.Height = 0;
		return size;
	}

	public static sw.Size InfinityIfNan(this sw.Size size)
	{
		if (double.IsNaN(size.Width))
			size.Width = double.PositiveInfinity;
		if (double.IsNaN(size.Height))
			size.Height = double.PositiveInfinity;
		return size;
	}

	public static sw.Size Add(this sw.Size size1, sw.Size size2)
	{
		return new sw.Size(size1.Width + size2.Width, size1.Height + size2.Height);
	}

	public static sw.Size Subtract(this sw.Size size1, sw.Size size2)
	{
		return new sw.Size(Math.Max(0, size1.Width - size2.Width), Math.Max(0, size1.Height - size2.Height));
	}

	public static sw.Point Add(this sw.Point point, sw.Point point2)
	{
		return new sw.Point(point.X + point2.X, point.Y + point2.Y);
	}

	public static sw.Point Add(this sw.Point point, sw.Size size2)
	{
		return new sw.Point(point.X + size2.Width, point.Y + size2.Height);
	}

	public static sw.Point Subtract(this sw.Point point, sw.Point point2)
	{
		return new sw.Point(point.X - point2.X, point.Y - point2.Y);
	}
	public static sw.Point Subtract(this sw.Point point, sw.Size size2)
	{
		return new sw.Point(point.X - size2.Width, point.Y - size2.Height);
	}

	public static sw.Point Divide(this sw.Point point, double factor)
	{
		return new sw.Point(point.X / factor, point.Y / factor);
	}
}
