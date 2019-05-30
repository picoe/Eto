using System;
using Eto.Forms;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;

namespace Eto.WinForms.Forms
{
	public class ScreenHandler : WidgetHandler<swf.Screen, Screen>, Screen.IHandler
	{
		float scale;

		public ScreenHandler (swf.Screen screen)
		{
			this.Control = screen;
			var form = new swf.Form ();
			var graphics = form.CreateGraphics ();
			scale = graphics.DpiY / 72f;
		}

		public float RealScale
		{
			get { return scale; }
		}

		public float Scale
		{
			get { return scale; }
		}

		public RectangleF Bounds
		{
			get { return Control.Bounds.ToEto (); }
		}

		public RectangleF WorkingArea
		{
			get { return Control.WorkingArea.ToEto (); }
		}

		public int BitsPerPixel
		{
			get { return Control.BitsPerPixel; }
		}

		public bool IsPrimary
		{
			get { return Control.Primary; }
		}
	}
}

