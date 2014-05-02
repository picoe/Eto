using Eto.Platform.Wpf.Forms.Controls;
using System;

namespace Eto.Test.Wpf
{
	class Startup
	{
		[STAThread]
		static void Main (string[] args)
		{
			var generator = new Eto.Platform.Wpf.Generator ();

			// don't use tiling for the direct drawing test
			Style.Add<DrawableHandler>("direct", handler => handler.AllowTiling = false);

			var app = new TestApplication (generator);
			app.Run (args);
		}

	}
}

