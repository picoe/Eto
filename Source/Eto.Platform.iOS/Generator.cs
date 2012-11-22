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
		

		public override IDisposable ThreadStart ()
		{
			return new NSAutoreleasePool ();
		}
		


	}
}

