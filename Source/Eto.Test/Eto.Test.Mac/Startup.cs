using Eto.Mac.Forms.Controls;
using Eto.Mac.Forms;
using Eto.Mac;
using System.Diagnostics;
#if XAMMAC2
using AppKit;
#else
using MonoMac.AppKit;
#endif

namespace Eto.Test.Mac
{
	class Startup
	{
		static void Main (string[] args)
		{
#if DEBUG && !XAMMAC2
			Debug.Listeners.Add (new ConsoleTraceListener ());
#endif
			AddStyles ();
			
			var generator = new Eto.Mac.Platform ();
			
			var app = new TestApplication (generator);

			// use this to use your own app delegate:
			// ApplicationHandler.Instance.AppDelegate = new MyAppDelegate();

			app.Run();
		}

		static void AddStyles ()
		{
			// support full screen mode!
			Style.Add<FormHandler> ("main", handler => {
				handler.Control.CollectionBehavior |= NSWindowCollectionBehavior.FullScreenPrimary;
			});

			Style.Add<ApplicationHandler> ("application", handler => {
				handler.EnableFullScreen ();
			});

			// other styles
			Style.Add<TreeGridViewHandler> ("sectionList", handler => {
				handler.ScrollView.BorderType = NSBorderType.NoBorder;
				handler.Control.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.SourceList;
			});

			Style.Add<ButtonToolItemHandler> (null, handler => {
				// use standard textured/round buttons, and make the image grayscale
				handler.UseStandardButton (grayscale: true);
			});

			Style.Add<ToolBarHandler> (null, handler => { 
				// change display mode or other options
				//handler.Control.DisplayMode = NSToolbarDisplayMode.Icon;
			});
		}
	}
}	

