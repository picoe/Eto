using System;

namespace Eto.WinForms.Forms;

#if NET9_0_OR_GREATER

public class ThemeHandler : WidgetHandler<swf.SystemColorMode, Theme, Theme.ICallback>, Theme.IHandler
{
	public string Name => Control.ToString();

	public ThemeStyle ThemeStyle => Control switch
	{
		swf.SystemColorMode.System => swf.Application.SystemColorMode switch
		{
			swf.SystemColorMode.Classic => ThemeStyle.Light,
			swf.SystemColorMode.Dark => ThemeStyle.Dark,
			_ => ThemeStyle.Light
		},
		swf.SystemColorMode.Classic => ThemeStyle.Light,
		swf.SystemColorMode.Dark => ThemeStyle.Dark,
		_ => ThemeStyle.Light, 
	};

	public ThemeHandler(swf.SystemColorMode mode = swf.SystemColorMode.System)
	{
		Control = mode;
	}
	
	public void SetTheme()
	{
		swf.Application.SetColorMode(Control);
	}
}

public class ThemesHandler : Themes.IHandler
{
	Theme _light;
	Theme _dark;
	Theme _system;
	public Theme Light => _light ??= new Theme(new ThemeHandler(swf.SystemColorMode.Classic));
	public Theme Dark => _dark ??= new Theme(new ThemeHandler(swf.SystemColorMode.Dark));
	public Theme System => _system ??= new Theme(new ThemeHandler(swf.SystemColorMode.System));
	
	public IEnumerable<Theme> GetThemes()
	{
		yield return System;
		yield return Light;
		yield return Dark;
	}
}


#endif