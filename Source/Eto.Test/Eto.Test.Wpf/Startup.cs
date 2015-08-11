using Eto.Wpf.Forms.Controls;
using System;

namespace Eto.Test.Wpf
{
	class Startup
	{
		[STAThread]
		static void Main(string[] args)
		{
			var platform = new Eto.Wpf.Platform();

			var app = new TestApplication(platform);
			app.TestAssemblies.Add(typeof(Startup).Assembly);
			app.Run();
		}

	}
}

