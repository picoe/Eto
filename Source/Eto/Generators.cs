using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto
{
	/// <summary>
	/// Constants for the standard platform generators
	/// </summary>
	public static class Generators
	{
		/// <summary>
		/// ID of the iOS platform
		/// </summary>
		public const string Ios = "ios";
		
		/// <summary>
		/// Assembly name of the iOS platform
		/// </summary>
		public const string IosAssembly = "Eto.Platform.iOS.Generator, Eto.Platform.iOS";

		/// <summary>
		/// ID of the GTK platform
		/// </summary>
		public const string Gtk = "gtk";

		/// <summary>
		/// Assembly name of the GTK platform
		/// </summary>
		public const string GtkAssembly = "Eto.Platform.GtkSharp.Generator, Eto.Platform.Gtk";

		/// <summary>
		/// ID of the GTK 3.0 platform
		/// </summary>
		public const string Gtk3 = "gtk3";

		/// <summary>
		/// Assembly name of the GTK platform
		/// </summary>
		public const string Gtk3Assembly = "Eto.Platform.GtkSharp.Generator, Eto.Platform.Gtk3";

		/// <summary>
		/// ID of the Mac OS X platform
		/// </summary>
		public const string Mac = "mac";

		/// <summary>
		/// Assembly name of the Mac OS X platform
		/// </summary>
		public const string MacAssembly = "Eto.Platform.Mac.Generator, Eto.Platform.Mac";

		/// <summary>
		/// ID of the Mac OS X platform
		/// </summary>
		public const string XamMac = "xammac";
		
		/// <summary>
		/// Assembly name of the Mac OS X platform
		/// </summary>
		public const string XamMacAssembly = "Eto.Platform.Mac.Generator, Eto.Platform.XamMac";
		
		/// <summary>
		/// ID of the Windows forms platform
		/// </summary>
		public const string Windows = "windows";

		/// <summary>
		/// Assembly name of the Windows Forms platform
		/// </summary>
		public const string WinAssembly = "Eto.Platform.Windows.Generator, Eto.Platform.Windows";

		/// <summary>
		/// ID of the WPF platform
		/// </summary>
		public const string Wpf = "wpf";

		/// <summary>
		/// Assembly name of the WPF platform
		/// </summary>
		public const string WpfAssembly = "Eto.Platform.Wpf.Generator, Eto.Platform.Wpf";
	}
}
