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
				var oldDpiAwareness = Win32.PerMonitorDpiSupported ? Win32.SetThreadDpiAwarenessContext(Win32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_v2) : Win32.DPI_AWARENESS_CONTEXT.NONE;
				var result = swf.Control.MousePosition.ScreenToLogical();
				if (oldDpiAwareness != Win32.DPI_AWARENESS_CONTEXT.NONE)
					Win32.SetThreadDpiAwarenessContext(oldDpiAwareness);
				return result;
			}
			set
			{
				var oldDpiAwareness = Win32.PerMonitorDpiSupported ? Win32.SetThreadDpiAwarenessContext(Win32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_v2) : Win32.DPI_AWARENESS_CONTEXT.NONE;
				swf.Cursor.Position = Point.Round(value.LogicalToScreen()).ToSD();
				if (oldDpiAwareness != Win32.DPI_AWARENESS_CONTEXT.NONE)
					Win32.SetThreadDpiAwarenessContext(oldDpiAwareness);
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
