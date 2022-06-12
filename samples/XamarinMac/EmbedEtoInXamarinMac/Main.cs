using AppKit;

namespace EmbedEtoInXamarinMac
{
	static class MainClass
	{
		static void Main(string[] args)
		{
			NSApplication.Init();

			// initialize eto forms after native app is initialized
			new Eto.Forms.Application(new Eto.Mac.Platform()).Attach();

			NSApplication.Main(args);
		}
	}
}
