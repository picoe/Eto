using System;
using Eto.Forms;

namespace EmbedGtkInEto
{
	static class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			new Application(new Eto.GtkSharp.Platform()).Run(new MainForm());
		}
	}
}

