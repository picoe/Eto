using Eto.Mac.Forms.Controls;
using Eto.Mac.Forms;
using Eto.Mac;
using System.Diagnostics;
using Eto.Drawing;
using Eto.Mac.Forms.ToolBar;
using Eto.Forms;

#if XAMMAC2
using AppKit;
#else
using MonoMac.AppKit;
#endif

namespace Eto.Test.Mac
{
	class Startup
	{
		static void Main(string[] args)
		{
			AddStyles();

			var platform = new Eto.Mac.Platform();
			
			var app = new TestApplication(platform);
			app.TestAssemblies.Add(typeof(Startup).Assembly);

			// use this to use your own app delegate:
			// ApplicationHandler.Instance.AppDelegate = new MyAppDelegate();

			app.Run();
		}

		static void AddStyles()
		{
			// support full screen mode!
			Style.Add<FormHandler>("main", handler =>
				{
					handler.Control.CollectionBehavior |= NSWindowCollectionBehavior.FullScreenPrimary;
				});

			Style.Add<ApplicationHandler>("application", handler =>
				{
					handler.EnableFullScreen();
				});

			// other styles
			Style.Add<TreeGridViewHandler>("sectionList", handler =>
				{
					handler.Border = BorderType.None;

					handler.Control.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.SourceList;
					handler.Control.FloatsGroupRows = false;

					handler.ShowGroups = true;
					handler.AllowGroupSelection = false;
				});

			Style.Add<ButtonToolItemHandler>(null, handler =>
			{
				// tint the images in grayscale
				handler.Tint = Colors.Gray;
			});
			Style.Add<CheckToolItemHandler>(null, handler =>
			{
				// tint the images in grayscale
				handler.Tint = Colors.Gray;
			});

			Style.Add<ToolBarHandler>(null, handler =>
				{ 
					// change display mode or other options
					//handler.Control.DisplayMode = NSToolbarDisplayMode.Icon;
				});

		}
	}
}

