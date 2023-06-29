using UIKit;
namespace Eto.iOS.Forms
{
	public class ScreensHandler : Screen.IScreensHandler
	{
		public System.Collections.Generic.IEnumerable<Screen> Screens
		{
			get { return UIScreen.Screens.Select(r => new Screen(new ScreenHandler(r))); }
		}

		public Screen PrimaryScreen
		{
			get { return new Screen(new ScreenHandler(UIScreen.MainScreen)); }
		}
	}
}

