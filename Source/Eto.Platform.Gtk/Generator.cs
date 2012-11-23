using System;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;
using Eto.IO;
using Eto.Platform.GtkSharp.Drawing;
using System.Threading;

namespace Eto.Platform.GtkSharp
{
	public class Generator : Eto.Generator
	{ 	
		public override string ID {
			get { return Generators.Gtk; }
		}
		
		public Generator ()
		{
			AddAssembly (typeof(Generator).Assembly);
			
			if (EtoEnvironment.Platform.IsWindows && Environment.Is64BitProcess)
				throw new NotSupportedException("Please compile/run GTK in x86 mode (32-bit) on windows");
			Gtk.Application.Init();
			
			Gdk.Threads.Enter ();
		}
	}
}
