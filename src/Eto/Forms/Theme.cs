namespace Eto.Forms;

/// <summary>
/// Represents a visual theme that can be applied to the application.
/// </summary>
/// <remarks>
/// Themes control the overall visual appearance of the application, such as light or dark mode.
/// Use <see cref="Application.Theme"/> to get or set the active theme,
/// and <see cref="Themes"/> to enumerate available themes.
/// </remarks>
[Handler(typeof(IHandler))]
public class Theme : Widget
{
	new IHandler Handler => (IHandler)base.Handler;

	/// <summary>
	/// Gets the display name of the theme.
	/// </summary>
	public string Name => Handler.Name;

	/// <summary>
	/// Gets the style category of the theme (System, Light, or Dark).
	/// </summary>
	public ThemeStyle ThemeStyle => Handler.ThemeStyle;

	/// <summary>
	/// Initializes a new instance of the <see cref="Theme"/> class with the specified handler.
	/// </summary>
	/// <param name="handler">The platform handler for this theme.</param>
	public Theme(IHandler handler) : base(handler)
	{
	}

	/// <summary>
	/// Handler interface for the <see cref="Theme"/> class.
	/// </summary>
	public new interface IHandler : Widget.IHandler
	{
		/// <summary>
		/// Gets the display name of the theme.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the style category of the theme.
		/// </summary>
		ThemeStyle ThemeStyle { get; }
	}

	/// <inheritdoc/>
	public override string ToString() => Name;

	/// <inheritdoc/>
	public override bool Equals(object obj)
	{
		return obj is Theme theme && theme.Name == Name && theme.ThemeStyle == ThemeStyle;
	}


	/// <summary>
	/// Determines whether two <see cref="Theme"/> instances are equal based on their name and style.
	/// </summary>
	/// <param name="a">The first theme to compare.</param>
	/// <param name="b">The second theme to compare.</param>
	/// <returns><c>true</c> if the themes are equal; otherwise, <c>false</c>.</returns>
	public static bool operator ==(Theme a, Theme b)
	{
		if (ReferenceEquals(a, b))
			return true;
		if (a is null || b is null)
			return false;
		return a.Equals(b);
	}
	
	/// <summary>
	/// Determines whether two <see cref="Theme"/> instances are not equal based on their name and style.
	/// </summary>
	/// <param name="a">The first theme to compare.</param>
	/// <param name="b">The second theme to compare.</param>
	/// <returns><c>true</c> if the themes are not equal; otherwise, <c>false</c>.</returns>
	public static bool operator !=(Theme a, Theme b) => !(a == b);

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		return Name.GetHashCode() ^ ThemeStyle.GetHashCode();
	}
}

/// <summary>
/// Specifies the visual style category of a <see cref="Theme"/>.
/// </summary>
public enum ThemeStyle
{
	/// <summary>
	/// Specifies the theme is considered a light style
	/// </summary>
	Light,
	/// <summary>
	/// Specifies the theme is considered a dark style
	/// </summary>
	Dark
}

/// <summary>
/// Provides access to the available themes for the current platform.
/// </summary>
[Handler(typeof(IHandler))]
public static class Themes
{
	static IHandler Handler => Platform.Instance.CreateShared<IHandler>();

	/// <summary>
	/// Gets all themes available on the current platform.
	/// </summary>
	public static IEnumerable<Theme> AllThemes => Handler.GetThemes();

	/// <summary>
	/// Gets the system theme, which follows the OS light/dark setting.
	/// </summary>
	public static Theme System => Handler.System;

	/// <summary>
	/// Gets the light theme.
	/// </summary>
	public static Theme Light => Handler.Light;

	/// <summary>
	/// Gets the dark theme.
	/// </summary>
	public static Theme Dark => Handler.Dark;

	/// <summary>
	/// Handler interface for the <see cref="Themes"/> class.
	/// </summary>
	public interface IHandler
	{
		/// <summary>
		/// Gets the system theme.
		/// </summary>
		Theme System { get; }

		/// <summary>
		/// Gets the light theme.
		/// </summary>
		Theme Light { get; }

		/// <summary>
		/// Gets the dark theme.
		/// </summary>
		Theme Dark { get; }

		/// <summary>
		/// Gets all themes supported by the current platform.
		/// </summary>
		IEnumerable<Theme> GetThemes();
	}
}

