using System;
using Eto;
using Eto.Test;

namespace Eto.Test.WinForms
{
	class Startup
	{
		[STAThread]
		static void Main (string [] args)
		{
			var generator = Generator.GetGenerator(Generators.Direct2DAssembly);
			var app = new TestApplication (generator);
			app.Run (args);
		}
	}
}

