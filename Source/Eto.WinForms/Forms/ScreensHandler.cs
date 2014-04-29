using Eto.Forms;
using System.Collections.Generic;
using sd = System.Drawing;
using swf = System.Windows.Forms;

namespace Eto.WinForms.Forms
{
	public class ScreensHandler : IScreens
	{
		public void Initialize ()
		{
		}

		public Widget Widget { get; set; }

		public Eto.Platform Platform { get; set; }

		public IEnumerable<Screen> Screens
		{
			get
			{
				foreach (var screen in swf.Screen.AllScreens)
				{
					yield return new Screen(Platform, new ScreenHandler(screen));
				}
			}
		}

		public Screen PrimaryScreen
		{
			get
			{
				var screen = swf.Screen.PrimaryScreen;
				return new Screen (Platform, new ScreenHandler (screen));
			}
		}
	}
}

