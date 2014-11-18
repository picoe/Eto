using Eto.Forms;
using System;

namespace EmbedWinFormsInEto
{
	static class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			new Application(new Eto.WinForms.Platform()).Run(new MainForm());
		}
	}
}
