using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using Eto.Test.Interface;
using Eto.Forms;

namespace Eto.Test.Mac
{
	class Startup
	{
		static void Main (string [] args)
		{
			AddStyles ();
			
			var generator = new Eto.Platform.Mac.Generator ();
			
			var app = new TestApplication (generator);
			app.Run (args);
			
		}
		
		static void AddStyles ()
		{
			// support full screen mode!
			Style.Add<Window, NSWindow> ("main", (widget, control) => {
				//control.CollectionBehavior |= NSWindowCollectionBehavior.FullScreenPrimary; // not in monomac/master yet..
			});
			
			Style.Add<Application, NSApplication> ("application", (widget, control) => {
				if (control.RespondsToSelector (new Selector ("presentationOptions:"))) {
					control.PresentationOptions |= NSApplicationPresentationOptions.FullScreen;
				}
			});

			// other styles
			Style.Add<ListBox, NSScrollView> ("sectionList", (widget, control) => {
				control.BorderType = NSBorderType.NoBorder;
				var table = control.DocumentView as NSTableView;
				table.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.SourceList;
			});
		}
	}
}	

