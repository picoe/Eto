using System;
using Eto.Drawing;

namespace Eto.Forms
{
	[Flags]
	public enum MouseButtons
	{
		None = 0x00,
		Primary = 0x01,
		Alternate = 0x02,
		Middle = 0x04
	}
	
	public class MouseEventArgs : EventArgs
	{
		public MouseEventArgs(MouseButtons buttons, Key modifiers, Point location,  int delta = 0)
		{
			this.Modifiers = modifiers;
			this.Buttons = buttons;
			this.Location = location;
			this.Pressure = 1.0f;
			this.Delta = delta;
		}
		
		public Key Modifiers { get; private set; }
		
		public MouseButtons Buttons { get; private set; }
		
		public Point Location { get; private set; }
		
		public bool Handled { get; set; }
		
		public float Pressure { get; set; }

        //
        // Summary:
        //     Gets a signed count of the number of detents the mouse wheel has rotated.
        //     A detent is one notch of the mouse wheel.
        //
        // Returns:
        //     A signed count of the number of detents the mouse wheel has rotated.
        public int Delta { get; set; }
	}
}

