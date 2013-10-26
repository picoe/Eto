using System;

namespace Eto.Test.Wpf
{
	class Startup
	{
		[STAThread]
		static void Main (string[] args)
		{
			var generator = new Eto.Platform.Wpf.Generator ();

			var app = new TestApplication (generator);
			app.Run (args);
		}

	}
}

