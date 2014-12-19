using System;
using UIKit;
using Eto.Forms;

namespace Eto.iOS.Forms
{
	public class ScreenHandler : WidgetHandler<UIScreen, Screen, Screen.ICallback>, Screen.IHandler
	{
		public ScreenHandler(UIScreen control)
		{
			Control = control;
		}

		public float Scale
		{
			get { return 1f; }
		}

		public float RealScale
		{
			get { return (float)Control.Scale; }
		}

		public int BitsPerPixel
		{
			get { return 32; }
		}

		public Eto.Drawing.RectangleF Bounds
		{
			get { return Control.Bounds.ToEto(); }
		}

		public Eto.Drawing.RectangleF WorkingArea
		{
			get { return Control.ApplicationFrame.ToEto(); }
		}

		public bool IsPrimary
		{
			get { return Control.Handle == UIScreen.MainScreen.Handle; }
		}
	}
}

