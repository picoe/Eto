using System;
using Eto.Forms;

namespace EmbedMonoMacInEto
{
	static class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			new Application(new Eto.Mac.Platform()).Run(new MainForm());
		}
	}
}
