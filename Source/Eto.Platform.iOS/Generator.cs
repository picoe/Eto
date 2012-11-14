using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Linq;
using Eto.Drawing;
using MonoTouch.CoreGraphics;
using Eto.Forms;

namespace Eto.Platform.iOS
{
	public class Generator : Eto.Generator
	{
		public const string GeneratorID = "ios";

		public override string ID {
			get { return GeneratorID; }
		}

		public Generator ()
		{
			AddAssembly (typeof(Generator).Assembly);
		}
		
		public override bool Supports<T> ()
		{
			var type = typeof(T);
			if (UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad) {
				// all iPad-only stuff is not supported on other idioms..
				if (type == typeof(ISplitter))
					return false;
				
			}
			return base.Supports<T> ();
		}
		
		public static Point GetLocation (UIView view, UIEvent theEvent)
		{
			var touches = theEvent.TouchesForView (view);
			var touch = touches.ToArray<UITouch> ().FirstOrDefault ();
			var loc = touch.LocationInView (view);
			loc.Y = view.Frame.Height - loc.Y;
			return loc.ToEtoPoint ();
		}
		
		
		public override IDisposable ThreadStart ()
		{
			return new NSAutoreleasePool ();
		}
		

		public static MouseEventArgs ConvertMouse (UIView view, NSSet touches, UIEvent evt)
		{
			if (touches.Count > 0) {
				UITouch touch = touches.ToArray<UITouch> () [0];
				var location = touch.LocationInView (view);
				return new MouseEventArgs (MouseButtons.Primary, Key.None, location.ToEtoPoint ());
			}
			return new MouseEventArgs (MouseButtons.Primary, Key.None, Point.Empty);
		}

	}
}

