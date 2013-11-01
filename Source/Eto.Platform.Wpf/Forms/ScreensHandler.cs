using Eto.Forms;
using System.Collections.Generic;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms
{
	public class ScreensHandler : IScreens
	{
		public void Initialize ()
		{
		}

		public Widget Widget { get; set; }

		public Eto.Generator Generator { get; set; }

		public IEnumerable<Screen> Screens
		{
			get
			{
				foreach (var screen in swf.Screen.AllScreens)
				{
					yield return new Screen(Generator, new ScreenHandler(screen));
				}
			}
		}

		public Screen PrimaryScreen
		{
			get
			{
				var screen = swf.Screen.PrimaryScreen;
				return new Screen (Generator, new ScreenHandler (screen));
			}
		}
	}
}

