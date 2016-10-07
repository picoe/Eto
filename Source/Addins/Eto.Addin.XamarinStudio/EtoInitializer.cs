using System;
using System.Linq;
using Eto.Forms;

namespace Eto.Addin.XamarinStudio
{
	public static class EtoInitializer
	{
		static readonly string AddinPlatform = Eto.Platforms.Gtk2;

		public static void Initialize()
		{
			if (Platform.Instance == null)
			{
				try
				{
					new Eto.Forms.Application(AddinPlatform).Attach();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"{ex}");
				}
				if (EtoEnvironment.Platform.IsMac)
				{
					var plat = Platform.Instance;
					if (!plat.IsMac)
					{
						// use some Mac handlers even when using Gtk platform as base
						plat.Add<Cursor.IHandler>(() => new Eto.Mac.Forms.CursorHandler());
					}
				}
			}
		}
	}
}

