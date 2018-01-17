using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace EmbedEtoInMonoMac
{
	static class Program
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

