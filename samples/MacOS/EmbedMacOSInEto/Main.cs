using System;
using AppKit;
using Eto.Forms;

namespace EmbedMacOSInEto
{
	static class MainClass
	{
		[STAThread]
		public static void Main(string[] args)
		{
			new Application(new Eto.Mac.Platform()).Run(new MainForm());
		}
	}
}
