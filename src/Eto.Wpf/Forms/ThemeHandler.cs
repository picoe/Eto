using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using Microsoft.Win32;

namespace Eto.Wpf.Forms;

/// <summary>
/// Controls WPF's text selection rendering mode at runtime.
/// </summary>
/// <remarks>
/// By default WPF draws the text selection as an adorner *over* the text, which ignores
/// <see cref="swc.Primitives.TextBoxBase.SelectionTextBrush"/> and hides the text entirely when
/// SelectionOpacity is 1. The modern (non-adorner) rendering draws the highlight behind the text
/// and repaints selected glyphs with SelectionTextBrush.
/// WPF caches the underlying AppContext switch on first use, so this helper also resets that
/// cache (via reflection) to allow toggling after text controls have already rendered.
/// </remarks>
static class TextSelectionRenderingSwitch
{
	const string SwitchName = "Switch.System.Windows.Controls.Text.UseAdornerForTextboxSelectionRendering";

	static bool? _previousValue;

	/// <summary>
	/// Enables non-adorner text selection rendering, remembering the previous setting.
	/// </summary>
	public static void Enable()
	{
		if (_previousValue == null)
		{
			// Some dependency property defaults (e.g. TextBoxBase.SelectionOpacity, which is
			// 0.4 for adorner mode but 1.0 for non-adorner mode) are chosen based on the
			// rendering mode when the control type is first initialized, and are frozen from
			// then on. Force that initialization NOW, before changing the mode, so the
			// defaults match the previous mode we may restore later — otherwise restoring
			// adorner rendering would paint the selection at the frozen opacity of 1.0,
			// hiding the selected text.
			System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(swc.Primitives.TextBoxBase).TypeHandle);
			System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(swc.PasswordBox).TypeHandle);

			// WPF's default is true (adorner-based) when the switch was never set
			_previousValue = AppContext.TryGetSwitch(SwitchName, out var value) ? value : true;
		}
		Apply(useAdorner: false);
	}

	/// <summary>
	/// Restores the setting that was in effect before <see cref="Enable"/> was called.
	/// </summary>
	public static void Restore()
	{
		if (_previousValue == null)
			return;
		Apply(_previousValue.Value);
		_previousValue = null;
	}

	static void Apply(bool useAdorner)
	{
		AppContext.SetSwitch(SwitchName, useAdorner);
		try
		{
			// WPF caches the switch value on first use; reset the cache so changing it
			// takes effect immediately. Selections re-render with the new mode when they
			// are next updated (e.g. focus/selection change or theme re-render).
			var switchesType = typeof(sw.FrameworkElement).Assembly.GetType("MS.Internal.FrameworkAppContextSwitches");
			var field = switchesType?.GetField("_useAdornerForTextboxSelectionRendering", BindingFlags.Static | BindingFlags.NonPublic);
			field?.SetValue(null, 0); // 0 = not yet cached
		}
		catch
		{
			// ignore; the switch still applies if set before the first text control renders
		}
	}
}

public class ResourceThemeHandler : ThemeHandler<List<Uri>>
{
	static ResourceThemeHandler()
	{
		Style.Add<Window>(null, StyleWindows);
	}

	public ResourceThemeHandler(string name, ThemeStyle style, IEnumerable<Uri> resourceUris = null)
	{
		Control = new List<Uri>(resourceUris ?? Enumerable.Empty<Uri>());
		ThemeStyle = style;
		Name = name;
	}

	public override ThemeStyle ThemeStyle { get; }

	public override string Name { get; }

	/// <summary>
	/// Gets or sets a value indicating that WPF's modern (non-adorner) text selection rendering
	/// should be enabled while this theme is active, so theme styles can use SelectionTextBrush
	/// with an opaque SelectionOpacity. With WPF's default adorner-based rendering, the selection
	/// is drawn over the text (hiding it completely when SelectionOpacity is 1) and
	/// SelectionTextBrush is ignored. The previous rendering mode is restored when switching
	/// to a different theme.
	/// </summary>
	public bool UseNonAdornerTextSelection { get; set; }

	public override IEnumerable<Uri> GetResourceUris() => Control;

	public override void SetTheme()
	{
#if NET9_0_OR_GREATER
		sw.Application.Current.ThemeMode = sw.ThemeMode.None;
#endif
		if (UseNonAdornerTextSelection)
			TextSelectionRenderingSwitch.Enable();
		SetWindowsTitleBarDarkMode(ThemeStyle == ThemeStyle.Dark);
	}

	public override void UnsetTheme()
	{
		if (UseNonAdornerTextSelection)
			TextSelectionRenderingSwitch.Restore();
	}

	private static void StyleWindows(Window window)
	{
		if (Application.Instance.Theme?.Handler is ResourceThemeHandler handler
			&& window.Handler is IWpfWindow wpfWindow && wpfWindow.Control != null)
		{
			if (wpfWindow.Control.IsLoaded)
			{
				handler.StyleWindow(wpfWindow.Control);
			}
			else
			{
				// If the window isn't initialized yet, we need to wait until it is before we can set the theme
				void OnLoaded(object sender, EventArgs e)
				{
					handler.StyleWindow(wpfWindow.Control);
					wpfWindow.Control.Loaded -= OnLoaded;
				}
				wpfWindow.Control.Loaded += OnLoaded;
			}
		}
	}

	/// <summary>
	/// Applies the theme to a single window's OS-drawn chrome, once it has an HWND. Called for every
	/// window as it loads, and for already-open windows when the theme is applied.
	/// </summary>
	protected virtual void StyleWindow(sw.Window window)
	{
		SetWindowDarkMode(window, ThemeStyle == ThemeStyle.Dark);
	}
}

public class PaletteTheme : Theme
{
	public PaletteTheme(string name, ThemeStyle style, Uri paletteUri, IEnumerable<Uri> additionalResourceUris = null)
		: base(new PaletteThemeHandler(name, style, paletteUri, additionalResourceUris))
	{
	}
}

public class ResourceTheme : Theme
{
	public ResourceTheme(string name, ThemeStyle style, IEnumerable<Uri> resourceUris = null)
		: base(new ResourceThemeHandler(name, style, resourceUris))
	{
	}
}


/// <summary>
/// A theme built from Eto.Wpf's reusable palette-driven control styles (themes/palette.xaml).
/// </summary>
/// <remarks>
/// The application only supplies a palette resource dictionary defining the Eto.Palette.*
/// colors and brushes; all control styles reference those via DynamicResource.
/// See test/Eto.Test.Wpf/themes/custom/MochaPalette.xaml in the Eto repository for a complete
/// example palette documenting the expected keys.
/// Non-adorner text selection rendering is enabled by default since the palette styles
/// use SelectionTextBrush with an opaque SelectionOpacity.
/// </remarks>
public class PaletteThemeHandler : ResourceThemeHandler
{
	public PaletteThemeHandler(string name, ThemeStyle style, Uri paletteUri, IEnumerable<Uri> additionalResourceUris = null)
		: base(name, style, GetUris(paletteUri, additionalResourceUris))
	{
		UseNonAdornerTextSelection = true;
	}

	static IEnumerable<Uri> GetUris(Uri paletteUri, IEnumerable<Uri> additionalResourceUris)
	{
		if (paletteUri == null)
			throw new ArgumentNullException(nameof(paletteUri));
		// The palette theme is merged FIRST so it acts as the lowest-priority layer:
		// it supplies the reusable control styles and sensible defaults for palette-tunable
		// keys (e.g. Eto.Palette.CornerRadius / *Padding). The application's palette is merged
		// AFTER it so a palette can override those defaults (WPF merged dictionaries are
		// last-wins), and any additional overrides are merged last so they win over everything.
		yield return AssemblyAbsoluteResourceDictionary.GetAbsolutePackUri("themes/palette.xaml");
		yield return paletteUri;
		if (additionalResourceUris != null)
		{
			// application-specific overrides, merged after the palette so they win
			foreach (var uri in additionalResourceUris)
				yield return uri;
		}
	}

	// Holds the SystemColors.*ColorKey overrides copied from the palette. Kept as a merged
	// dictionary so it can be removed wholesale when switching away from this theme.
	sw.ResourceDictionary _systemColorKeys;

	static swm.Color? FindColor(string key) => sw.Application.Current?.TryFindResource(key) as swm.Color?;

	/// <summary>
	/// Color the OS-drawn window frame is tinted with, so the title bar matches the window content
	/// rather than the system chrome. Each part falls back to the palette color it sits next to,
	/// which a palette can override with the matching Eto.Palette.TitleBar*.Color key.
	/// </summary>
	static (swm.Color? caption, swm.Color? text, swm.Color? border) GetCaptionColors() => (
		FindColor("Eto.Palette.TitleBarBackground.Color") ?? FindColor("Eto.Palette.Background.Color"),
		FindColor("Eto.Palette.TitleBarForeground.Color") ?? FindColor("Eto.Palette.Foreground.Color"),
		FindColor("Eto.Palette.TitleBarBorder.Color") ?? FindColor("Eto.Palette.Border.Color"));

	protected override void StyleWindow(sw.Window window)
	{
		base.StyleWindow(window);
		var (caption, text, border) = GetCaptionColors();
		SetWindowCaptionColors(window, caption, text, border);
	}

	/// <summary>
	/// Mirrors the SystemColors *BrushKey* overrides in themes/palette.xaml onto the matching
	/// *ColorKey* resources.
	/// </summary>
	/// <remarks>
	/// The brush keys are redirected to the palette in XAML because a <see cref="swm.SolidColorBrush"/>
	/// is a <see cref="sw.DependencyObject"/> whose Color can hold a DynamicResource pointing at the
	/// runtime-merged palette. A <see cref="swm.Color"/> resource entry cannot reference another
	/// resource (neither DynamicResource nor StaticResource parse into a Color), so the *ColorKey*
	/// entries — which some generic control templates consume directly (e.g. EtoCustomComboBox*,
	/// TreeToggleButton.*) — are copied here in code once the palette has actually been merged.
	/// </remarks>
	public override void ThemeResourcesMerged()
	{
		base.ThemeResourcesMerged();

		var app = sw.Application.Current;
		if (app == null)
			return;

		// Drop any previous set (e.g. if the same theme instance is re-applied without an
		// intervening UnsetTheme) so we never accumulate duplicate dictionaries.
		if (_systemColorKeys != null)
			app.Resources.MergedDictionaries.Remove(_systemColorKeys);

		var dict = new sw.ResourceDictionary();

		void Map(sw.ResourceKey colorKey, string paletteColorKey)
		{
			// Only override when the palette actually supplies the color; otherwise leave the
			// key to fall through to the system default.
			if (app.TryFindResource(paletteColorKey) is swm.Color color)
				dict[colorKey] = color;
		}

		// Backgrounds
		Map(sw.SystemColors.WindowColorKey, "Eto.Palette.Background.Color");
		Map(sw.SystemColors.ControlColorKey, "Eto.Palette.Surface.Color");
		Map(sw.SystemColors.ControlLightColorKey, "Eto.Palette.Background.Color");
		Map(sw.SystemColors.InfoColorKey, "Eto.Palette.Background.Color");

		// Foregrounds
		Map(sw.SystemColors.ControlTextColorKey, "Eto.Palette.Foreground.Color");
		Map(sw.SystemColors.WindowTextColorKey, "Eto.Palette.Foreground.Color");
		Map(sw.SystemColors.InfoTextColorKey, "Eto.Palette.Foreground.Color");
		Map(sw.SystemColors.GrayTextColorKey, "Eto.Palette.SubtleForeground.Color");

		// Selection / highlight
		Map(sw.SystemColors.HighlightColorKey, "Eto.Palette.Accent.Color");
		Map(sw.SystemColors.HighlightTextColorKey, "Eto.Palette.AccentForeground.Color");

		// Borders
		Map(sw.SystemColors.ControlDarkColorKey, "Eto.Palette.Border.Color");
		Map(sw.SystemColors.ControlDarkDarkColorKey, "Eto.Palette.Border.Color");
		Map(sw.SystemColors.WindowFrameColorKey, "Eto.Palette.Border.Color");
		Map(sw.SystemColors.ActiveBorderColorKey, "Eto.Palette.Accent.Color");
		Map(sw.SystemColors.InactiveBorderColorKey, "Eto.Palette.Border.Color");

		// Menus
		Map(sw.SystemColors.MenuColorKey, "Eto.Palette.Background.Color");
		// the menu bar blends into the window by default, see themes/palette.xaml
		Map(sw.SystemColors.MenuBarColorKey, "Eto.Palette.Background.Color");
		Map(sw.SystemColors.MenuTextColorKey, "Eto.Palette.Foreground.Color");
		Map(sw.SystemColors.MenuHighlightColorKey, "Eto.Palette.Accent.Color");

		// Hover / hot-track: consumed by generic templates (e.g. TreeToggleButton) but has no
		// matching SystemColors *BrushKey* override in palette.xaml, so it would otherwise fall
		// through to the system accent. Map it to the palette accent to match.
		Map(sw.SystemColors.HotTrackColorKey, "Eto.Palette.Accent.Color");

		_systemColorKeys = dict;
		app.Resources.MergedDictionaries.Add(dict);

		// Windows already on screen when the theme was applied don't get a Loaded event, so tint
		// their frames here — the palette colors this reads only exist once it has been merged,
		// which is why this can't happen in SetTheme().
		var (caption, text, border) = GetCaptionColors();
		SetWindowsCaptionColors(caption, text, border);
	}

	public override void UnsetTheme()
	{
		base.UnsetTheme();
		if (_systemColorKeys != null)
		{
			sw.Application.Current?.Resources.MergedDictionaries.Remove(_systemColorKeys);
			_systemColorKeys = null;
		}
		// Hand the window frames back to the system, so switching to a theme that doesn't tint
		// them (the default WPF/Fluent themes) doesn't leave this palette's colors behind.
		SetWindowsCaptionColors(null, null, null);
	}
}

public interface IThemeHandler : Theme.IHandler
{
	IEnumerable<Uri> GetResourceUris();
	void SetTheme();
	void UnsetTheme();

	/// <summary>
	/// Called after the theme's resource dictionaries (see <see cref="GetResourceUris"/>) have been
	/// merged into the application resources, so handlers can apply overrides that depend on values
	/// supplied by those dictionaries (e.g. copying palette colors into SystemColors keys).
	/// </summary>
	void ThemeResourcesMerged();
}

public abstract class ThemeHandler<T> : WidgetHandler<T, Theme, Theme.ICallback>, IThemeHandler
{

	public abstract string Name { get; }

	public abstract ThemeStyle ThemeStyle { get; }

	public virtual IEnumerable<Uri> GetResourceUris() => Enumerable.Empty<Uri>();

	public virtual void SetTheme()
	{
	}

	public virtual void UnsetTheme()
	{
	}

	public virtual void ThemeResourcesMerged()
	{
	}

	/// <summary>
	/// Runs <paramref name="apply"/> against every window of the application, deferring any window
	/// that has no HWND yet until it gets one.
	/// </summary>
	/// <remarks>
	/// A theme is often applied while the main window exists as a <see cref="sw.Window"/> but has not
	/// been shown, and the DWM frame attributes these callers set need a real HWND — applied then,
	/// they would silently do nothing and the window would come up with system chrome. Deferring to
	/// SourceInitialized covers that; a window created after this point is handled by the
	/// <see cref="ResourceThemeHandler.StyleWindow"/> hook instead.
	/// </remarks>
	protected static void ApplyToWindows(Action<sw.Window> apply)
	{
		var app = sw.Application.Current;
		if (app == null || app.Dispatcher != swt.Dispatcher.CurrentDispatcher)
			return;

		foreach (sw.Window window in app.Windows)
		{
			if (new System.Windows.Interop.WindowInteropHelper(window).Handle != IntPtr.Zero)
			{
				apply(window);
				continue;
			}
			var target = window;
			void OnSourceInitialized(object sender, EventArgs e)
			{
				target.SourceInitialized -= OnSourceInitialized;
				apply(target);
			}
			target.SourceInitialized += OnSourceInitialized;
		}
	}

	protected static void SetWindowsTitleBarDarkMode(bool useDarkMode)
	{
		ApplyToWindows(window => SetWindowDarkMode(window, useDarkMode));
	}

	protected static void SetWindowDarkMode(sw.Window window, bool useDarkMode)
	{
		var hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
		int useDarkModeValue = useDarkMode ? 1 : 0;
		Win32.DwmSetWindowAttribute(hwnd, Win32.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkModeValue, sizeof(int));
	}

	/// <summary>
	/// Tints the OS-drawn window frame — caption background, caption text and border — so it matches
	/// the theme instead of the system chrome. Pass null for any part to restore its system default.
	/// </summary>
	/// <remarks>
	/// This needs Windows 11 (build 22000) or later; on earlier versions DWM rejects the attributes
	/// and the frame keeps its system look, with only <see cref="SetWindowDarkMode"/> applying. The
	/// window must already have an HWND (i.e. be loaded), otherwise there is nothing to tint.
	/// </remarks>
	protected static void SetWindowCaptionColors(sw.Window window, swm.Color? caption, swm.Color? text, swm.Color? border)
	{
		var hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
		if (hwnd == IntPtr.Zero)
			return;

		void Set(int attribute, swm.Color? color)
		{
			var value = color is swm.Color c ? Win32.MakeColorRef(c.R, c.G, c.B) : Win32.DWMWA_COLOR_DEFAULT;
			Win32.DwmSetWindowAttribute(hwnd, attribute, ref value, sizeof(int));
		}

		Set(Win32.DWMWA_CAPTION_COLOR, caption);
		Set(Win32.DWMWA_TEXT_COLOR, text);
		Set(Win32.DWMWA_BORDER_COLOR, border);
	}

	/// <summary>
	/// Applies <see cref="SetWindowCaptionColors"/> to the application's existing windows — for when
	/// the theme changes while they are shown, or before they are (see <see cref="ApplyToWindows"/>).
	/// Windows created later are handled by <see cref="ResourceThemeHandler.StyleWindow"/>.
	/// </summary>
	protected static void SetWindowsCaptionColors(swm.Color? caption, swm.Color? text, swm.Color? border)
	{
		ApplyToWindows(window => SetWindowCaptionColors(window, caption, text, border));
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
		if (Control == sw.ThemeMode.None)
			SetWindowsTitleBarDarkMode(false);
	}
}

#endif

public class ThemesHandler : Themes.IHandler
{
	Theme _light;
	Theme _dark;
	Theme _system;
	Theme _none;
	
	public static ThemesHandler Instance => Platform.Instance.CreateShared<Themes.IHandler>() as ThemesHandler;
	
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