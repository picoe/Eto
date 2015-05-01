using System;
using Eto;
using Eto.Test;

namespace Eto.Test.Direct2D
{
	class Startup
	{
		[STAThread]
		static void Main(string[] args)
		{
			var platform = new Eto.Direct2D.Platform();
			var app = new TestApplication(platform, typeof(Startup).Assembly);
			app.Run();
		}
	}
}

