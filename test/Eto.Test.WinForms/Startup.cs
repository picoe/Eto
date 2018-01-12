using System;
using Eto;
using Eto.Test;

namespace Eto.Test.WinForms
{
	class Startup
	{
		[STAThread]
		static void Main(string[] args)
		{
			var platform = Platform.Get(Platforms.WinForms);
			var app = new TestApplication(platform);
			app.TestAssemblies.Add(typeof(Startup).Assembly);
			app.Run();
		}
	}
}

