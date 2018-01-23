using Eto.Addin.VisualStudio.Editor;
using Eto.Designer;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Addin.VisualStudio.Wizards
{
	public static class EtoInitializer
	{
		public static void Initialize()
		{
			if (Platform.Instance == null)
			{
				var platform = new Eto.Wpf.Platform();
				// uncomment to use app domains
				platform.LoadAssembly(typeof(PlatformInitializer).Assembly);
				new Application(platform).Attach();

				Eto.Designer.Builders.BaseCompiledInterfaceBuilder.InitializeAssembly = typeof(EtoInitializer).Assembly.FullName;
			}
		}
	}
}
