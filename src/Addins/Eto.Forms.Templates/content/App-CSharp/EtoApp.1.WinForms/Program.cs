using System;
using Eto.Forms;

namespace EtoApp.1.WinForms
{
	class MainClass
	{
		[STAThread]
		public static void Main(string[] args)
		{
			new Application(Eto.Platforms.WinForms).Run(new MainForm());
		}
	}
}
