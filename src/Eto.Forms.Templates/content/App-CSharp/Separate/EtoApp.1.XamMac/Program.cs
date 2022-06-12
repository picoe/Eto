using AppKit;
using Eto.Forms;

namespace EtoApp._1.XamMac
{
	class Program
	{
		static void Main(string[] args)
		{
			new Application(Eto.Platforms.XamMac2).Run(new MainForm());
		}
	}
}
