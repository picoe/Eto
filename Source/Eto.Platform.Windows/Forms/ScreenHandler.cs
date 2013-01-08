using System;
using Eto.Forms;
using sd = System.Drawing;
using swf = System.Windows.Forms;

namespace Eto.Platform.Windows.Forms
{
	public class ScreenHandler : WidgetHandler<swf.Screen, Screen>, IScreen
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
	}
}

