using swi = System.Windows.Input;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms
{
	public class MouseHandler : IMouse
	{
		public Widget Widget { get; set; }

		public void Initialize ()
		{
		}

		public Eto.Generator Generator { get; set; }

		public PointF Position
		{
			get { return swf.Control.MousePosition.ToEto (); }
		}

		public MouseButtons Buttons
		{
			get {
				MouseButtons buttons = MouseButtons.None;
				if (swi.Mouse.LeftButton == swi.MouseButtonState.Pressed)
					buttons |= MouseButtons.Primary;
				if (swi.Mouse.MiddleButton == swi.MouseButtonState.Pressed)
					buttons |= MouseButtons.Middle;
				if (swi.Mouse.RightButton == swi.MouseButtonState.Pressed)
					buttons |= MouseButtons.Alternate;
				return buttons;
			}
		}
	}
}
