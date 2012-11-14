using System;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;
using Eto.IO;
using MonoMac.AppKit;
using Eto.Platform.Mac.Drawing;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using Eto.Platform.Mac.IO;
using System.Threading;

namespace Eto.Platform.Mac
{
	public class Generator : Eto.Generator
	{ 	
		public override string ID {
			get { return Generators.Mac; }
		}

		public Generator ()
		{
			AddAssembly (typeof (Generator).Assembly);
		}
		
		public static Point GetLocation (NSView view, NSEvent theEvent)
		{
			var loc = view.ConvertPointFromView (theEvent.LocationInWindow, null);
			if (!view.IsFlipped)
				loc.Y = view.Frame.Height - loc.Y;
			return loc.ToEtoPoint ();
		}
		
		public override IDisposable ThreadStart ()
		{
			return new NSAutoreleasePool ();
		}


		public static MouseEventArgs GetMouseEvent (NSView view, NSEvent theEvent)
		{
			var pt = Generator.GetLocation (view, theEvent);
			Key modifiers = KeyMap.GetModifiers (theEvent);
			MouseButtons buttons = KeyMap.GetMouseButtons (theEvent);
			return new MouseEventArgs (buttons, modifiers, pt);
		}
		
		public static void SetSizeWithAuto (NSView view, Size size)
		{
			var newSize = view.Frame.Size;
			if (size.Width >= 0)
				newSize.Width = size.Width;
			if (size.Height >= 0)
				newSize.Height = size.Height;
			view.SetFrameSize (newSize);
		}
	}
}
