using Eto.Addin.VisualStudio.Editor;
using Eto.Designer;
using Eto.Forms;
using Eto.Wpf.Forms;
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

				Style.Add<FormHandler>("themed", h => ThemeWindow(h.Control));
				Style.Add<DialogHandler>("themed", h => ThemeWindow(h.Control));
				var platform = new Eto.Wpf.Platform();
				// uncomment to use app domains
				platform.LoadAssembly(typeof(EtoInitializer).Assembly);
				new Application(platform).Attach();

				Eto.Designer.Builders.BaseCompiledInterfaceBuilder.InitializeAssembly = typeof(EtoInitializer).Assembly.FullName;
			}
		}

		private static void ThemeWindow(System.Windows.Window w)
		{
			w.Resources.MergedDictionaries.Add(new System.Windows.ResourceDictionary { Source = new Uri("pack://application:,,,/Eto.Addin.VisualStudio;component/theme/WindowStyles.xaml", UriKind.RelativeOrAbsolute) });
		}
	}
}
