using System;
using Eto.Forms;

namespace EtoApp._1.Wpf
{
	class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			var app = new Application(Eto.Platforms.Wpf);
			// Wpf doesn't follow the system light/dark theme unless asked to
			app.Theme = Themes.System;
			app.Run(new MainForm());
		}
	}
}
