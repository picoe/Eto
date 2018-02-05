using System;

namespace Eto
{
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
		/// Type of the GTK 2 platform
		/// </summary>
		[Obsolete("Gtk2 platform is obsolete, please use Platforms.Gtk instead.")]
		public static readonly string Gtk2 = "Eto.GtkSharp.Platform, Eto.Gtk2";
		/// <summary>
		/// Type of the GTK 3 platform
		/// </summary>
		[Obsolete("Gtk3 platform is obsolete, please use Platforms.Gtk instead.")]
		public static readonly string Gtk3 = "Eto.GtkSharp.Platform, Eto.Gtk3";
		/// <summary>
		/// Type of the Mac OS X platform
		/// </summary>
		public static readonly string Mac = "Eto.Mac.Platform, Eto.Mac";
		/// <summary>
		/// Type of the Mac OS X platform on 64-bit mono
		/// </summary>
		public static readonly string Mac64 = "Eto.Mac.Platform, Eto.Mac64";
		/// <summary>
		/// Type of the Xamarin.Mac v1 OS X platform
		/// </summary>
		public static readonly string XamMac = "Eto.Mac.Platform, Eto.XamMac";
		/// <summary>
		/// Type of the Xamarin.Mac v2 OS X platform
		/// </summary>
		public static readonly string XamMac2 = "Eto.Mac.Platform, Eto.XamMac2";
		/// <summary>
		/// Type of the Windows forms platform
		/// </summary>
		public static readonly string WinForms = "Eto.WinForms.Platform, Eto.WinForms";
		/// <summary>
		/// ID of the WPF platform
		/// </summary>
		public static readonly string Wpf = "Eto.Wpf.Platform, Eto.Wpf";
		/// <summary>
		/// ID of the Android platform
		/// </summary>
		public static readonly string Android = "Eto.Android.Platform, Eto.Android";
	}
}
