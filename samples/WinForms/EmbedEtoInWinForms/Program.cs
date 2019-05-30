using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmbedEtoInWinForms
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// initialize eto forms after native app is initialized
			new Eto.Forms.Application(new Eto.WinForms.Platform()).Attach();

			Application.Run(new Form1());
		}
	}
}
