namespace Eto;

/// <summary>
/// Constants for the standard Generator generators
/// </summary>
public static class Platforms
{
	/// <summary>
	/// Type of the Direct2D platform
	/// </summary>
	public static readonly string Direct2D = "Eto.Direct2D.Platform, Eto.Direct2D";
	/// <summary>
	/// Type of the iOS platform
	/// </summary>
	public static readonly string Ios = "Eto.iOS.Platform, Eto.iOS";
	/// <summary>
	/// Type of the GTK platform
	/// </summary>
	public static readonly string Gtk = "Eto.GtkSharp.Platform, Eto.Gtk";
	/// <summary>
	/// Type of the macOS platform on .NET 6+ or mono
	/// </summary>
	public static readonly string Mac64 = "Eto.Mac.Platform, Eto.Mac64";
	/// <summary>
	/// Type of the Xamarin.Mac v2 macOS platform on mono
	/// </summary>
	public static readonly string XamMac2 = "Eto.Mac.Platform, Eto.XamMac2";
	/// <summary>
	/// Type of the macOS platform on .NET 6.0.2xx+ SDK
	/// </summary>
	public static readonly string macOS = "Eto.Mac.Platform, Eto.macOS";
	/// <summary>
	/// Type of the Windows Forms platform
	/// </summary>
	public static readonly string WinForms = "Eto.WinForms.Platform, Eto.WinForms";
	/// <summary>
	/// Type of the WPF platform
	/// </summary>
	public static readonly string Wpf = "Eto.Wpf.Platform, Eto.Wpf";
	/// <summary>
	/// Type of the Android platform
	/// </summary>
	public static readonly string Android = "Eto.Android.Platform, Eto.Android";
}