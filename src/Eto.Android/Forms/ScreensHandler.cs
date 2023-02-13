using System.Collections.Generic;
using Android.Runtime;
using Eto.Forms;
using Eto.Drawing;

using aa = Android.App;
using ac = Android.Content;
using av = Android.Views;
using au = Android.Util;

namespace Eto.Android.Forms
{
	internal class ScreensHandler : Screen.IScreensHandler
	{
		public ScreensHandler()
		{
			var Metrics = new au.DisplayMetrics();
			var WindowManager = Platform.AppContext.GetSystemService(ac.Context.WindowService).JavaCast<av.IWindowManager>();
			WindowManager.DefaultDisplay.GetMetrics(Metrics);

			var SH = (int)(Metrics.HeightPixels / Metrics.Density);
			var SW = (int)(Metrics.WidthPixels / Metrics.Density);

			var Bounds = new RectangleF(0, 0, SW, SH);

			var Scale = 160f / 72f;

			PrimaryScreen = new Screen(new ScreenHandler(Scale, Metrics.Density, Bounds, Bounds, 16, true));
		}

		public Widget Widget { get; set; }

		public IEnumerable<Screen> Screens => new[] { PrimaryScreen };

		public Screen PrimaryScreen { get; }
	}
}
