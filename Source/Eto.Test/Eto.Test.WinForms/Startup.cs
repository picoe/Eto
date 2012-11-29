using System;
using Eto;
using Eto.Misc;
using Eto.Test;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.WinForms
{
	class Startup
	{
		[STAThread]
		static void Main (string [] args)
		{
			var generator = Generator.GetGenerator ("Eto.Platform.Windows.Generator, Eto.Platform.Windows");
			
			var app = new TestApplication (generator);
			app.Run (args);
		}

	}
}

