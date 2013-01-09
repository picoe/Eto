using System;
using Eto.Forms;
using sw = System.Windows;
using sd = System.Drawing;
using swm = System.Windows.Media;
using swf = System.Windows.Forms;

namespace Eto.Platform.Wpf.Forms
{
	public class ScreenHandler : WidgetHandler<swf.Screen, Screen>, IScreen
	{
		float scale;

		public ScreenHandler (sw.Window window)
		{
			var source = sw.PresentationSource.FromVisual (window);
			Control = GetCurrentScreen (window);
			scale = (float)(source.CompositionTarget.TransformToDevice.M22 * 92.0 / 72.0);
		}

		public ScreenHandler (swf.Screen screen)
		{
			Control = screen;
			var form = new swf.Form ();
			var graphics = form.CreateGraphics ();
			scale = graphics.DpiY / 72f;
		}

		static swf.Screen GetCurrentScreen (sw.Window window)
		{
			var centerPoint = new sd.Point ((int)(window.Left + window.Width / 2), (int)(window.Top + window.Height / 2));
			foreach (var s in swf.Screen.AllScreens) {
				if (s.Bounds.Contains (centerPoint))
					return s;
			}
			return null;
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

