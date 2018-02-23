using Eto;
using Eto.Addin.VisualStudio;
using Eto.Designer;
using Eto.Drawing;
using Eto.Wpf;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

[assembly: ExportHandler(typeof(IPlatformTheme), typeof(PlatformThemeHandler))]

namespace Eto.Addin.VisualStudio
{
	class PlatformThemeHandler : IPlatformTheme
	{
		public Color ProjectBackground => VSColorTheme.GetThemedColor(ThemedDialogColors.WindowPanelColorKey).ToEto();

		public Color ProjectForeground => VSColorTheme.GetThemedColor(ThemedDialogColors.WindowPanelTextColorKey).ToEto();
		public Color ProjectDialogBackground => VSColorTheme.GetThemedColor(EnvironmentColors.NewProjectBackgroundColorKey).ToEto();

		public Color ErrorForeground => VSColorTheme.GetThemedColor(EnvironmentColors.DesignerBackgroundBrushKey).ToEto();

		public Color SummaryBackground => VSColorTheme.GetThemedColor(ThemedDialogColors.WizardFooterColorKey).ToEto();

		public Color SummaryForeground => VSColorTheme.GetThemedColor(ThemedDialogColors.WindowPanelTextColorKey).ToEto();

		public Color DesignerBackground => VSColorTheme.GetThemedColor(EnvironmentColors.DesignerBackgroundColorKey).ToEto();

		public Color DesignerPanel => SystemColors.ControlBackground;

		public IEnumerable<PlatformColor> AllColors
		{
			get
			{
				return
					GetColors(typeof(ThemedDialogColors))
					.Union(GetColors(typeof(EnvironmentColors)))
					.Union(GetColors(typeof(CommonControlsColors)))
					;
			}
		}

		IEnumerable<PlatformColor> GetColors(Type type)
		{
			var props = type.GetProperties(BindingFlags.Static | BindingFlags.Public);
			foreach (var prop in props)
			{
				if (prop.Name.EndsWith("BrushKey"))
					continue;
				var key = prop.GetValue(null) as ThemeResourceKey;
				if (key == null)
					continue;
				var color = VSColorTheme.GetThemedColor(key).ToEto();
				yield return new PlatformColor { Name = $"{type.Name}.{prop.Name}", Color = color };
			}
		}
	}
}
