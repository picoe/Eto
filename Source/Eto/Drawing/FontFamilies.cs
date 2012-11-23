using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Drawing
{
	/// <summary>
	/// Defines generic font families that can be used on all systems
	/// </summary>
	/// <remarks>
	/// The font families here may correspond to certain fonts on each system, depending on the platform.
	/// 
	/// These font families are "guaranteed" to be available, mainly by using pre-installed fonts on each
	/// platform.
	/// </remarks>
	public static class FontFamilies
	{
		/// <summary>
		/// Gets the name of the monospace system family name
		/// </summary>
		/// <remarks>
		/// Not intended to be used directly, use <see cref="Monospace"/>. Used by platform handlers
		/// to determine which system font family to get
		/// </remarks>
		public const string MonospaceFamilyName = "Eto.Monospace";

		/// <summary>
		/// Gets a monospace font family
		/// </summary>
		public static FontFamily Monospace
		{
			get { return Fonts.GetSystemFontFamily (MonospaceFamilyName); }
		}

		/// <summary>
		/// Gets the name of a sans-serif system family name
		/// </summary>
		/// <remarks>
		/// Not intended to be used directly, use <see cref="Sans"/>. Used by platform handlers
		/// to determine which system font family to get
		/// </remarks>
		public const string SansFamilyName = "Eto.Sans";

		/// <summary>
		/// Gets a sans-serif font family
		/// </summary>
		public static FontFamily Sans
		{
			get { return Fonts.GetSystemFontFamily (SansFamilyName); }
		}

		/// <summary>
		/// Gets the name of a serif system family name
		/// </summary>
		/// <remarks>
		/// Not intended to be used directly, use <see cref="Serif"/>. Used by platform handlers
		/// to determine which system font family to get
		/// </remarks>
		public const string SerifFamilyName = "Eto.Serif";

		/// <summary>
		/// Gets a serif font family
		/// </summary>
		public static FontFamily Serif
		{
			get { return Fonts.GetSystemFontFamily (SerifFamilyName); }
		}
	}
}
