using System;
using Eto.Forms;
using sw = System.Windows;
using sd = System.Drawing;
using swm = System.Windows.Media;
using swf = System.Windows.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms
{
	public class ScreenHandler : WidgetHandler<swf.Screen, Screen>, Screen.IHandler
	{
		float? realScale;
		sw.Window window;

		public ScreenHandler(sw.Window window)
		{
			Control = GetCurrentScreen(window);
			this.window = window;
		}

		public ScreenHandler(swf.Screen screen)
		{
			Control = screen;
		}

		float GetRealScale()
		{
			if (realScale != null)
				return realScale.Value;

			if (window != null)
			{
				var source = sw.PresentationSource.FromVisual(window);
				if (source != null)
				{
					realScale = (float)source.CompositionTarget.TransformToDevice.M22;
					window = null;
				}
			}

			if (realScale == null)
			{
				using (var form = new swf.Form { Bounds = Control.Bounds })
				using (var graphics = form.CreateGraphics())
				{
					realScale = graphics.DpiY / 96f;
				}
			}
			return realScale ?? 1f;
		}

		static swf.Screen GetCurrentScreen(sw.Window window)
		{
			var centerPoint = new sd.Point((int)(window.Left + window.ActualWidth / 2), (int)(window.Top + window.ActualHeight / 2));
			foreach (var s in swf.Screen.AllScreens)
			{
				if (s.Bounds.Contains(centerPoint))
					return s;
			}
			return swf.Screen.PrimaryScreen;
		}

		public float RealScale
		{
			get { return GetRealScale() * Scale; }
		}

		public float Scale
		{
			get { return 96f / 72f; }
		}

		public RectangleF Bounds
		{
			get { return (RectangleF)Control.Bounds.ToEto() / GetRealScale(); }
		}

		public RectangleF WorkingArea
		{
			get { return (RectangleF)Control.WorkingArea.ToEto() / GetRealScale(); }
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