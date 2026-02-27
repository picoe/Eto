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
			get => ((PointF)swf.Cursor.Position.ToEto()) / Win32.SystemDpi;
			set => swf.Cursor.Position = (value * Win32.SystemDpi).ToSDPoint();
		}

		public MouseButtons Buttons
		{
			get { return swf.Control.MouseButtons.ToEto(); }
		}
	}
}
