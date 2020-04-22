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
			get => swf.Control.MousePosition.ScreenToLogical();
			set => swf.Cursor.Position = Point.Round(value.LogicalToScreen()).ToSD();
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
