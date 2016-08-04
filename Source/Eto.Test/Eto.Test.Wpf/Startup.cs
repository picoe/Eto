using Eto.Wpf.Forms.Controls;
using System;
using System.Windows.Media;
using System.Reflection;

namespace Eto.Test.Wpf
{
	class Startup
	{
		[STAThread]
		static void Main(string[] args)
		{
			var platform = new Eto.Wpf.Platform();
			
			// uncomment to test using CefSharp for the WebView.
			//platform.LoadAssembly("Eto.Wpf.CefSharp");

			// optional - enables GDI text display mode
			/*
			Style.Add<Eto.Wpf.Forms.FormHandler>(null, handler => TextOptions.SetTextFormattingMode(handler.Control, TextFormattingMode.Display));
			Style.Add<Eto.Wpf.Forms.DialogHandler>(null, handler => TextOptions.SetTextFormattingMode(handler.Control, TextFormattingMode.Display));
			*/

			var app = new TestApplication(platform);
			app.TestAssemblies.Add(typeof(Startup).Assembly);
			app.Run();
		}
	}
}

