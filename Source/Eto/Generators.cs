using System;


namespace Eto
{
	/// <summary>
	/// Constants for the standard Generator generators
	/// </summary>
	[Obsolete("Use Platforms instead. ID's are no longer defined in the core, just types")]
	public static class Generators
	{
		/// <summary>
		/// ID of the Direct2D platform
		/// </summary>
		public const string Direct2D = "Direct2D";

		/// <summary>
		/// Assembly name of the Direct2D platform
		/// </summary>
		public const string Direct2DAssembly = "Eto.Direct2D.Platform, Eto.Direct2D";

		/// <summary>
		/// ID of the iOS platform
		/// </summary>
		public const string Ios = "ios";
		
		/// <summary>
		/// Assembly name of the iOS platform
		/// </summary>
		public const string IosAssembly = "Eto.iOS.Platform, Eto.iOS";

		/// <summary>
		/// ID of the GTK platform
		/// </summary>
		public const string Gtk = "gtk";

		/// <summary>
		/// Assembly name of the GTK platform
		/// </summary>
		public const string GtkAssembly = "Eto.GtkSharp.Platform, Eto.Gtk2";

		/// <summary>
		/// ID of the GTK 3.0 platform
		/// </summary>
		public const string Gtk3 = "gtk3";

		/// <summary>
		/// Assembly name of the GTK platform
		/// </summary>
		public const string Gtk3Assembly = "Eto.GtkSharp.Platform, Eto.Gtk3";

		/// <summary>
		/// ID of the Mac OS X platform
		/// </summary>
		public const string Mac = "mac";

		/// <summary>
		/// Assembly name of the Mac OS X platform
		/// </summary>
		public const string MacAssembly = "Eto.Mac.Platform, Eto.Mac";

		/// <summary>
		/// ID of the Mac OS X platform
		/// </summary>
		public const string XamMac = "xammac";
		
		/// <summary>
		/// Assembly name of the Mac OS X platform
		/// </summary>
		public const string XamMacAssembly = "Eto.Mac.Platform, Eto.XamMac";
		
		/// <summary>
		/// ID of the Windows forms platform
		/// </summary>
		public const string Windows = "windows";

		/// <summary>
		/// Assembly name of the Windows Forms platform
		/// </summary>
		public const string WinAssembly = "Eto.WinForms.Platform, Eto.WinForms";

		/// <summary>
		/// ID of the WPF platform
		/// </summary>
		public const string Wpf = "wpf";

		/// <summary>
		/// Assembly name of the WPF platform
		/// </summary>
		public const string WpfAssembly = "Eto.Wpf.Platform, Eto.Wpf";
		
		/// <summary>
		/// ID of the Android platform
		/// </summary>
		public const string Android = "android";

		/// <summary>
		/// Assembly name of the Android platform
		/// </summary>
		public const string AndroidAssembly = "Eto.Android.Platform, Eto.Android";
	}
}
