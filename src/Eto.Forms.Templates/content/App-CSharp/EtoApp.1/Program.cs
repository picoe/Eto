using System;
using Eto.Forms;
using Eto.Drawing;

namespace EtoApp._1
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			var app = new Application(Eto.Platform.Detect);
			// Wpf doesn't follow the system light/dark theme unless asked to
			if (app.Platform.IsWpf)
				app.Theme = Themes.System;
			app.Run(new MainForm());
		}
	}
}
