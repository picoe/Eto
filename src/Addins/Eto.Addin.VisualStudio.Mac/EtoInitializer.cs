using System;
using System.Linq;
using Eto.Forms;
using Eto.Designer;
using System.Reflection;

namespace Eto.Addin.VisualStudio.Mac
{
	public static class EtoInitializer
	{
		static bool initialized;
		public static void Initialize()
		{
			if (initialized)
				return;

			initialized = true;

			try
			{
				var platform = Platform.Instance;
				if (platform == null)
				{
					platform = new Eto.Mac.Platform();
					Platform.Initialize(platform);
				}

				platform.LoadAssembly(typeof(EtoInitializer).Assembly);

				if (Application.Instance == null)
					new Eto.Forms.Application().Attach();

			}
			catch (Exception ex)
			{
				Console.WriteLine($"{ex}");
			}
		}
	}
}

