using System;
using System.Linq;
using Eto.Forms;
using Eto.Designer;
using System.Reflection;

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
					var platform = new Eto.Mac.Platform();

					platform.LoadAssembly(typeof(EtoInitializer).Assembly);

					new Eto.Forms.Application(platform).Attach();

					var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(r => r.GetName().Name.StartsWith("Xamarin")).ToArray();
					
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

