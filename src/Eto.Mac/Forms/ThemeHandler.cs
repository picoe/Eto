using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eto.Mac.Forms;

public class ThemeHandler : WidgetHandler<NSAppearance, Theme>, Theme.IHandler
{
	public ThemeHandler(NSAppearance appearance)
	{
		Control = appearance;
	}
	
	public string Name
	{
		get
		{
			var name = Control?.Name.ToString() ?? Application.Instance.Localize(Widget, "System");
			if (name.StartsWith("NSAppearanceName"))
				return name.Substring("NSAppearanceName".Length);
			return name;
		}
	}

	public ThemeStyle ThemeStyle
	{
		get
		{
			string name;
			if (Control == null)
				name = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle") ?? string.Empty;
			else
				name = Control.Name.ToString();
			return name.ToLowerInvariant().Contains("dark") ? ThemeStyle.Dark : ThemeStyle.Light;
		}
	}
	
	public override bool Equals(object obj)
	{
		if (obj is ThemeHandler themeHandler)
		{
			if (Control == null && themeHandler.Control == null)
				return true;
			return Control.Equals(themeHandler.Control);
		}
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return Control?.GetHashCode() ?? 0;
	}
}

public class ThemesHandler : Themes.IHandler
{
	Theme _system, _light, _dark;
	public Theme System => _system ??= new Theme(new ThemeHandler(null));
	public Theme Light => _light ??= new Theme(new ThemeHandler(NSAppearance.GetAppearance(NSAppearance.NameAqua)));
	public Theme Dark => _dark ??= new Theme(new ThemeHandler(NSAppearance.GetAppearance(NSAppearance.NameDarkAqua)));
	
	List<Theme> _themes;

	public IEnumerable<Theme> GetThemes() => _themes ??= new List<Theme>
	{
		System,
		Light,
		Dark,
		new Theme(new ThemeHandler(NSAppearance.GetAppearance(NSAppearance.NameVibrantLight))),
		new Theme(new ThemeHandler(NSAppearance.GetAppearance(NSAppearance.NameVibrantDark)))
	};
}
