using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms
{
	public class ScreenHandler : WidgetHandler<Gdk.Display, Screen>, IScreen
	{
		Gdk.Screen screen;
		public ScreenHandler (Gdk.Display display)
		{
			this.Control = display;
			this.screen = display.DefaultScreen;
		}

		public float RealScale
		{
			get { return (float)screen.Resolution / 72f; }
		}

		public float Scale
		{
			get { return (float)screen.Resolution / 72f; }
		}
	}
}

