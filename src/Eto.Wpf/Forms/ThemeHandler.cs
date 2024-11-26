using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Eto.Wpf.Forms;

public class ResourceThemeHandler : ThemeHandler<List<Uri>>
{
	public ResourceThemeHandler(string name, ThemeStyle style, IEnumerable<Uri> resourceUris = null)
	{
		Control = new List<Uri>(resourceUris ?? Enumerable.Empty<Uri>());
		ThemeStyle = style;
		Name = name;
	}

	public override ThemeStyle ThemeStyle { get; }

	public override string Name { get; }


	public override IEnumerable<Uri> GetResourceUris() => Control;
}

public interface IThemeHandler : Theme.IHandler
{
	IEnumerable<Uri> GetResourceUris();
	void SetTheme();
}

public abstract class ThemeHandler<T> : WidgetHandler<T, Theme, Theme.ICallback>, IThemeHandler
{

	public abstract string Name { get; }

	public abstract ThemeStyle ThemeStyle { get; }

	public virtual IEnumerable<Uri> GetResourceUris() => Enumerable.Empty<Uri>();

	public virtual void SetTheme()
	{
	}

	public void Register()
	{
		if (Platform.Instance.CreateShared<Themes.IHandler>() is ThemesHandler themesHandler)
		{
			themesHandler.Themes.Add(new Theme(this));
		}
	}
}

#if NET9_0_OR_GREATER

public class FluentThemeHandler : ThemeHandler<sw.ThemeMode>
{
	public FluentThemeHandler() : this(sw.ThemeMode.System)
	{
	}
	public FluentThemeHandler(sw.ThemeMode mode)
	{
		Control = mode;
	}

	public override string Name => Control.ToString();

	public override ThemeStyle ThemeStyle
	{
		get
		{
			if (Control == sw.ThemeMode.System)
				return ApplicationHandler.GetSystemThemeStyle();
			if (Control == sw.ThemeMode.Light)
				return ThemeStyle.Light;
			if (Control == sw.ThemeMode.Dark)
				return ThemeStyle.Dark;
			return ThemeStyle.Light;
		}
	}

	public override IEnumerable<Uri> GetResourceUris()
	{
		if (Control == sw.ThemeMode.None)
		{
			yield return AssemblyAbsoluteResourceDictionary.GetAbsolutePackUri("themes/none.xaml");
		}
		else
		{
			yield return new Uri("pack://application:,,,/PresentationFramework.Fluent;component/Themes/Fluent.xaml");
			yield return AssemblyAbsoluteResourceDictionary.GetAbsolutePackUri("themes/fluent.xaml");
		}
	}

	public override void SetTheme()
	{
		sw.Application.Current.ThemeMode = Control;
	}
}

#endif

public class ThemesHandler : Themes.IHandler
{
	Theme _light;
	Theme _dark;
	Theme _system;
	Theme _none;
	public Theme Light
	{
		get => _light ??= CreateTheme(ThemeStyle.Light);
		set => _light = value;
	}
	
	public Theme Dark
	{
		get => _dark ??= CreateTheme(ThemeStyle.Dark);
		set => _dark = value;
	}
	public Theme System
	{
		get => _system ??= CreateSystemTheme();
		set => _system = value;
	}
	public Theme None
	{
		get => _none ??= CreateTheme(null);
		set => _none = value;
	}

	static Theme _noneTheme;
	
	public static Theme GetNone()
	{
		if (_noneTheme == null)
		{
			var theme = new ResourceThemeHandler("None", ThemeStyle.Light);
			theme.Control.Add(AssemblyAbsoluteResourceDictionary.GetAbsolutePackUri("themes/none.xaml"));
			_noneTheme = new Theme(theme);
		}	
		return _noneTheme;
	}

	Theme CreateTheme(ThemeStyle? style)
	{
#if NET9_0_OR_GREATER
		var mode = style switch {
			ThemeStyle.Light => sw.ThemeMode.Light,
			ThemeStyle.Dark => sw.ThemeMode.Dark,
			_ => sw.ThemeMode.None
		};
		return new Theme(new FluentThemeHandler(mode));
#else
		return GetNone();
#endif
	}
	
	Theme CreateSystemTheme()
	{
#if NET9_0_OR_GREATER
		return new Theme(new FluentThemeHandler(sw.ThemeMode.System));
#else
		return GetNone();
#endif
	}

	List<Theme> _themes;

	public List<Theme> Themes => _themes ??= GetDefaultThemes().ToList();

	public IEnumerable<Theme> GetThemes() => Themes;
	
	IEnumerable<Theme> GetDefaultThemes()
	{
#if NET9_0_OR_GREATER
		yield return System;
		yield return Light;
		yield return Dark;
		yield return None;
#else
		yield return GetNone();
#endif	
	}
}