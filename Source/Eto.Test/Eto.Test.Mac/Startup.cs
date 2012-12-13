using System;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using Eto.Forms;
using Eto.Platform.Mac.Forms.Controls;
using Eto.Platform.Mac.Forms;
using Eto.Platform.Mac;
using Eto.Drawing;
using System.Diagnostics;
using MonoMac.CoreGraphics;

namespace Eto.Test.Mac
{
	class Startup
	{
		static void Main (string [] args)
		{
#if DEBUG
			Debug.Listeners.Add (new ConsoleTraceListener());
#endif
			AddStyles ();
			
			var generator = new Eto.Platform.Mac.Generator ();
			
			var app = new TestApplication (generator);


			var sw = new Stopwatch();
			var count = 1000000;

			var instantiator = Matrix.Instantiator ();
			var matrixInstantiator = Matrix.MatrixInstantiator ();
			sw.Start();
			for (var i = 0; i < count; ++i)
			{
				//var matrix = instantiator ();
				var matrix = matrixInstantiator();
				//var matrix = new Matrix ();
				//matrix.Scale (10, 10);
			}
			
			var e1 = sw.Elapsed;

			sw.Restart();
			
			for (var i = 0; i < count; ++i)
			{
				//var matrix = activator();
				//var matrix = instantiator ();
				//CGAffineTransform m;
				var matrix = CGAffineTransform.MakeIdentity();
				//matrix.Scale (10, 10);
				// alternately var m = System.Windows.Media.Matrix.Identity();
			}
			
			var e2 = sw.Elapsed;

			Console.WriteLine ("Time: eto: {0}, direct: {1}, Diff: {2}", e1.TotalMilliseconds, e2.TotalMilliseconds, (e1.TotalSeconds / e2.TotalSeconds));



			// use this to use your own app delegate:
			// ApplicationHandler.Instance.AppDelegate = new MyAppDelegate();
			app.Run (args);
			
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
		}
	}
}	

