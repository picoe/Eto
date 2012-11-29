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
		
		
		public override IDisposable ThreadStart ()
		{
			return new NSAutoreleasePool ();
		}
	}
}
