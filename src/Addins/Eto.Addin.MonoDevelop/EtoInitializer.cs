using System;
using System.Linq;
using Eto.Forms;
using Eto.Designer;

namespace Eto.Addin.MonoDevelop
{
	public static class EtoInitializer
	{
		public static void Initialize()
		{
			if (Platform.Instance == null)
			{
				try
				{
					var platform = new Eto.GtkSharp.Platform();

					platform.LoadAssembly(typeof(EtoInitializer).Assembly);

					new Eto.Forms.Application(platform).Attach();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"{ex}");
				}
#if Mac
				if (EtoEnvironment.Platform.IsMac)
				{
					var plat = Platform.Instance;
					if (!plat.IsMac)
					{
						// use some Mac handlers even when using Gtk platform as base
						plat.Add<Cursor.IHandler>(() => new Eto.Mac.Forms.CursorHandler());
					}
				}
#endif
			}
		}
	}
}

