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
		public MouseEventArgs(MouseButtons buttons, Key modifiers, Point location, SizeF? delta = null)
		{
			this.Modifiers = modifiers;
			this.Buttons = buttons;
			this.Location = location;
			this.Pressure = 1.0f;
			this.Delta = delta ?? SizeF.Empty;
		}
		
		public Key Modifiers { get; private set; }
		
		public MouseButtons Buttons { get; private set; }
		
		public Point Location { get; private set; }
		
		public bool Handled { get; set; }
		
		public float Pressure { get; set; }

        public SizeF Delta { get; set; }
	}
}

