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
		public const string MonospaceFamilyName = "monospace";

		/// <summary>
		/// Gets a monospace font family
		/// </summary>
		/// <param name="generator">Generator to get the font</param>
		/// <returns>A font family instance for the monospace font</returns>
		public static FontFamily Monospace(Generator generator = null)
		{
			return new FontFamily (generator, MonospaceFamilyName);
		}

		/// <summary>
		/// Gets the name of a sans-serif system family name
		/// </summary>
		/// <remarks>
		/// Not intended to be used directly, use <see cref="Sans"/>. Used by platform handlers
		/// to determine which system font family to get
		/// </remarks>
		public const string SansFamilyName = "sans-serif";

		/// <summary>
		/// Gets a sans-serif font family
		/// </summary>
		/// <param name="generator">Generator to get the font</param>
		/// <returns>A font family instance for the sans font</returns>
		public static FontFamily Sans(Generator generator = null)
		{
			return new FontFamily (generator, SansFamilyName);
		}

		/// <summary>
		/// Gets the name of a serif system family name
		/// </summary>
		/// <remarks>
		/// Not intended to be used directly, use <see cref="Serif"/>. Used by platform handlers
		/// to determine which system font family to get
		/// </remarks>
		public const string SerifFamilyName = "serif";

		/// <summary>
		/// Gets a serif font family
		/// </summary>
		/// <param name="generator">Generator to get the font</param>
		/// <returns>A font family instance for the serif font</returns>
		public static FontFamily Serif(Generator generator = null)
		{
			return new FontFamily (generator, SerifFamilyName);
		}

		/// <summary>
		/// Gets a cursive font family
		/// </summary>
		/// <param name="generator">Generator to get the font</param>
		/// <returns>A font family instance for the cursive font</returns>
		public static FontFamily Cursive (Generator generator = null)
		{
			return new FontFamily (generator, CursiveFamilyName);
		}

		/// <summary>
		/// Name of the cursive system family name
		/// </summary>
		/// <remarks>
		/// Not intended to be used directly, use <see cref="Cursive"/>. Used by platform handlers
		/// to determine which system font family to get
		/// </remarks>
		public const string CursiveFamilyName = "cursive";

		/// <summary>
		/// Gets a fantasy font family
		/// </summary>
		/// <param name="generator">Generator to get the font</param>
		/// <returns>A font family instance for the fantasy font</returns>
		public static FontFamily Fantasy(Generator generator = null)
		{
			return new FontFamily (generator, FantasyFamilyName);
		}

		/// <summary>
		/// Name of the fantasy system family name
		/// </summary>
		/// <remarks>
		/// Not intended to be used directly, use <see cref="Fantasy"/>. Used by platform handlers
		/// to determine which system font family to get
		/// </remarks>
		public const string FantasyFamilyName = "fantasy";

	}
}
