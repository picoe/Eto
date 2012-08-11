using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using Eto.Forms;
using Eto.Platform.Mac.Forms.Controls;
using Eto.Platform.Mac.Forms;
using Eto.Platform.Mac;

namespace Eto.Test.Mac
{
	class Startup
	{
		static void Main (string [] args)
		{
			AddStyles ();
			
			var generator = new Eto.Platform.Mac.Generator ();
			
			var app = new TestApplication (generator);

			// use this to use your own app delegate:
			// ApplicationHandler.Instance.AppDelegate = new MyAppDelegate();
			app.Run (args);
			
		}
		
		static void AddStyles ()
		{
			// support full screen mode!
			Style.Add<Window, NSWindow> ("main", (widget, control) => {
				control.CollectionBehavior |= NSWindowCollectionBehavior.FullScreenPrimary;
			});
			
			Style.Add<Application, NSApplication> ("application", (widget, control) => {
				if (control.RespondsToSelector (new Selector ("presentationOptions:"))) {
					control.PresentationOptions |= NSApplicationPresentationOptions.FullScreen;
				}
			});

			// other styles
			Style.AddHandler<TreeGridViewHandler> ("sectionList", (handler) => {
				handler.ScrollView.BorderType = NSBorderType.NoBorder;
				handler.Control.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.SourceList;
			});
		}
	}
}	

