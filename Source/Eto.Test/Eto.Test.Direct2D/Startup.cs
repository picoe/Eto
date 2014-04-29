using System;
using Eto;
using Eto.Test;

namespace Eto.Test.Direct2D
{
	class Startup
	{
		[STAThread]
		static void Main (string [] args)
		{
			var generator = Platform.Get(Platforms.Direct2D);
			var app = new TestApplication (generator);
			app.Run (args);
		}
	}
}

