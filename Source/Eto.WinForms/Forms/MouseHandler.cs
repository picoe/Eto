using Eto.Drawing;
using Eto.Forms;
using swf = System.Windows.Forms;

namespace Eto.WinForms.Forms
{
	public class MouseHandler : IMouse
	{
		public Widget Widget { get; set; }

		public void Initialize ()
		{
		}

		public Eto.Platform Platform { get; set; }

		public PointF Position
		{
			get { return swf.Control.MousePosition.ToEto (); }
		}

		public MouseButtons Buttons
		{
			get { return swf.Control.MouseButtons.ToEto (); }
		}
	}
}
