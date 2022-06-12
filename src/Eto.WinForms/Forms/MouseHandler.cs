using Eto.Drawing;
using Eto.Forms;
using swf = System.Windows.Forms;

namespace Eto.WinForms.Forms
{
	public class MouseHandler : Mouse.IHandler
	{
		public Widget Widget { get; set; }

		public void Initialize()
		{
		}

		public void SetCursor(Cursor cursor) => swf.Cursor.Current = cursor.ToSwf();

		public Eto.Platform Platform { get; set; }

		public PointF Position
		{
			get { return swf.Cursor.Position.ToEto(); }
			set { swf.Cursor.Position = value.ToSDPoint(); }
		}

		public MouseButtons Buttons
		{
			get { return swf.Control.MouseButtons.ToEto(); }
		}
	}
}
