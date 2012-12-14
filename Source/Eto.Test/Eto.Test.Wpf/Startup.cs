using System;
using Eto;
using Eto.Misc;
using Eto.Forms;
using Eto.Drawing;
using System.Diagnostics;
using Eto.Platform.Wpf.Drawing;
using System.Threading;
using System.Collections.Generic;

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

