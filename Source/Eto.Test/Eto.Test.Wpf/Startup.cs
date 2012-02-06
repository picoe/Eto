using System;
using Eto;
using Eto.Misc;

namespace Eto.Test.Wpf
{
	class Startup
	{
		[STAThread]
		static void Main (string [] args)
		{
			var generator = Generator.GetGenerator ("Eto.Platform.Wpf.Generator, Eto.Platform.Wpf");

			var app = new TestApplication (generator);
			app.Run (args);
		}
	}
}

