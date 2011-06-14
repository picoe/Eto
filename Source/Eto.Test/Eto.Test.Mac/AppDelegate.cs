using System;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Eto.Forms;

namespace Eto.Test.Mac
{
	public partial class AppDelegate : Eto.Platform.Mac.AppDelegate
	{
		
		// do any OS X - specific file open/launch handling here
		
		public override bool ApplicationShouldHandleReopen (NSApplication sender, bool hasVisibleWindows)
		{
			// show main window when the application button is clicked
			if (!hasVisibleWindows) {
				var form = Application.Instance.MainForm;
				if (form != null)
					form.Show ();
			}
			return true;
		}
		
	}
}

