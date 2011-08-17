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
			var generator = new Eto.Platform.Mac.Generator();
			
			var app = new TestApplication(generator);
			app.Run ();
			
		}
	}
}	

