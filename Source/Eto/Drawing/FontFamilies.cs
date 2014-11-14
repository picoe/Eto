
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
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class FontFamilies
	{
		/// <summary>
		/// Gets the name of the monospace system family name
		/// </summary>
		/// <remarks>
		/// Not intended to be used directly, use <see cref="FontFamilies.Monospace"/>. Used by platform handlers
		/// to determine which system font family to get
		/// </remarks>
		public const string MonospaceFamilyName = "MONOSPACE";

		/// <summary>
		/// Gets a monospace font family
		/// </summary>
		/// <returns>A font family instance for the monospace font</returns>
		public static FontFamily Monospace
		{
			get { return new FontFamily(MonospaceFamilyName); }
		}

		/// <summary>
		/// Gets the name of a sans-serif system family name
		/// </summary>
		/// <remarks>
		/// Not intended to be used directly, use <see cref="FontFamilies.Sans"/>. Used by platform handlers
		/// to determine which system font family to get
		/// </remarks>
		public const string SansFamilyName = "SANS-SERIF";

		/// <summary>
		/// Gets a sans-serif font family
		/// </summary>
		/// <returns>A font family instance for the sans font</returns>
		public static FontFamily Sans
		{
			get { return new FontFamily(SansFamilyName); }
		}

		/// <summary>
		/// Gets the name of a serif system family name
		/// </summary>
		/// <remarks>
		/// Not intended to be used directly, use <see cref="FontFamilies.Serif"/>. Used by platform handlers
		/// to determine which system font family to get
		/// </remarks>
		public const string SerifFamilyName = "SERIF";

		/// <summary>
		/// Gets a serif font family
		/// </summary>
		/// <returns>A font family instance for the serif font</returns>
		public static FontFamily Serif
		{
			get { return new FontFamily(SerifFamilyName); }
		}

		/// <summary>
		/// Gets a cursive font family
		/// </summary>
		/// <returns>A font family instance for the cursive font</returns>
		public static FontFamily Cursive
		{
			get { return new FontFamily(CursiveFamilyName); }
		}

		/// <summary>
		/// Name of the cursive system family name
		/// </summary>
		/// <remarks>
		/// Not intended to be used directly, use <see cref="FontFamilies.Cursive"/>. Used by platform handlers
		/// to determine which system font family to get
		/// </remarks>
		public const string CursiveFamilyName = "CURSIVE";

		/// <summary>
		/// Gets a fantasy font family
		/// </summary>
		/// <returns>A font family instance for the fantasy font</returns>
		public static FontFamily Fantasy
		{
			get { return new FontFamily(FantasyFamilyName); }
		}

		/// <summary>
		/// Name of the fantasy system family name
		/// </summary>
		/// <remarks>
		/// Not intended to be used directly, use <see cref="FontFamilies.Fantasy"/>. Used by platform handlers
		/// to determine which system font family to get
		/// </remarks>
		public const string FantasyFamilyName = "FANTASY";

	}
}
