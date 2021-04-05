using Eto.Addin.VisualStudio.Windows.Editor;
using Eto.Designer;
using Eto.Forms;
using Eto.Wpf.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Addin.VisualStudio.Windows.Wizards
{
	public static class EtoInitializer
	{
		static bool initialized;
		public static void Initialize()
		{
			if (initialized)
				return;

			initialized = true;

			var platform = Platform.Instance;
			if (platform == null)
			{
				platform = new Eto.Wpf.Platform();
				Platform.Initialize(platform);
			}

			Style.Add<FormHandler>("eto.vstheme", h => ThemeWindow(h.Control));
			Style.Add<DialogHandler>("eto.vstheme", h => ThemeWindow(h.Control));

			platform.LoadAssembly(typeof(EtoInitializer).Assembly);

			if (Application.Instance == null)
				new Eto.Forms.Application().Attach();

			Eto.Designer.Builders.BaseCompiledInterfaceBuilder.InitializeAssembly = typeof(EtoInitializer).Assembly.FullName;
		}

		private static void ThemeWindow(System.Windows.Window w)
		{
			w.Resources.MergedDictionaries.Add(new System.Windows.ResourceDictionary { Source = new Uri("pack://application:,,,/Eto.Addin.VisualStudio.Windows;component/theme/WindowStyles.xaml", UriKind.RelativeOrAbsolute) });
		}
	}
}
