using swi = System.Windows.Input;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms
{
	public class MouseHandler : Mouse.IHandler
	{
		public Widget Widget { get; set; }

		public void Initialize()
		{
		}

		public PointF Position
		{
			get
			{
				var screen = swf.Screen.FromPoint(swf.Control.MousePosition);
				var oldDpiAwareness = Win32.SetThreadDpiAwarenessContextSafe(Win32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_v2);
				var result = swf.Control.MousePosition;
				if (oldDpiAwareness != Win32.DPI_AWARENESS_CONTEXT.NONE)
					Win32.SetThreadDpiAwarenessContextSafe(oldDpiAwareness);
				return result.ScreenToLogical(screen);
			}
			set
			{
				var pos = value.LogicalToScreen();
				var oldDpiAwareness = Win32.SetThreadDpiAwarenessContextSafe(Win32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_v2);
				swf.Cursor.Position = Point.Round(pos).ToSD();
				if (oldDpiAwareness != Win32.DPI_AWARENESS_CONTEXT.NONE)
					Win32.SetThreadDpiAwarenessContextSafe(oldDpiAwareness);
			}
		}

		public static int s_CursorSetCount;

		public void SetCursor(Cursor cursor)
		{
			swi.Mouse.SetCursor(cursor.ToWpf());
			s_CursorSetCount++;
		}

		public MouseButtons Buttons => swf.Control.MouseButtons.ToEto();
	}
}
