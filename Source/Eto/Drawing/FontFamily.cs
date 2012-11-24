using System;
using System.Collections.Generic;

namespace Eto.Drawing
{
	/// <summary>
	/// Interface for a <see cref="FontFamily"/> handler
	/// </summary>
	public interface IFontFamily : IInstanceWidget
	{
		/// <summary>
		/// Gets the name of the font family
		/// </summary>
		/// <remarks>
		/// This should be the same as what is used to create new instances of a font family using the <see cref="Create"/> method
		/// </remarks>
		string Name { get; }

		/// <summary>
		/// Gets an enumeration of the typefaces supported by this font family
		/// </summary>
		IEnumerable<FontTypeface> Typefaces { get; }

		/// <summary>
		/// Creates a new instance of a font family with a given name
		/// </summary>
		/// <param name="familyName">Name of the font family to create this instance for</param>
		void Create (string familyName);
	}

	/// <summary>
	/// Specifies a family for a <see cref="Font"/> object
	/// </summary>
	/// <remarks>
	/// A font family defines the overall look of the font, such as "Times New Roman", "Helvetica", etc.
	/// 
	/// Each family consists of one or more <see cref="Typefaces"/>, which define the variations of each font family.
	/// The variations can include Light, Bold, Italic, Oblique, etc.  Only the styles in <see cref="FontStyle"/> are 
	/// discoverable, other than looking at the <see cref="FontTypeface.Name"/> for hints as to what the variation will look like.
	/// </remarks>
	public class FontFamily : InstanceWidget, IEquatable<FontFamily>
	{
		new IFontFamily Handler { get { return (IFontFamily)base.Handler; } }

		/// <summary>
		/// Gets the name of this font family.
        /// 
        /// This can differ from the name returned by the handler.
		/// </summary>
        public string Name { get; set; }

		/// <summary>
		/// Gets an enumeration of the one or more supported typefaces for this font family
		/// </summary>
		public IEnumerable<FontTypeface> Typefaces { get { return Handler.Typefaces; } }

		/// <summary>
		/// Gets a generic monospace font family that works across all platforms
		/// </summary>
		[Obsolete("Use FontFamilies.Monospace")]
		public static readonly FontFamily Monospace = FontFamilies.Monospace;

		/// <summary>
		/// Gets a generic sans-serif font family that works across all platforms
		/// </summary>
		[Obsolete ("Use FontFamilies.Sans")]
		public static readonly FontFamily Sans = FontFamilies.Sans;

		/// <summary>
		/// Gets a generic serif font family that works across all platforms
		/// </summary>
		[Obsolete ("Use FontFamilies.Serif")]
		public static readonly FontFamily Serif = FontFamilies.Serif;

		/// <summary>
		/// Initializes a new instance of the FontFamily class with the specified handler
		/// </summary>
		/// <remarks>
		/// Used by platform implementations to create instances of the FontFamily class directly
		/// </remarks>
		/// <param name="generator">Generator for this instance</param>
		/// <param name="handler">Handler to use</param>
		public FontFamily (Generator generator, IFontFamily handler, string name = null)
			: base (generator, handler, true)
		{
            this.Name = name ?? handler.Name;
		}

		/// <summary>
		/// Initializes a new instance of the FontFamily class with the given font <paramref name="familyName"/>
		/// </summary>
		/// <param name="familyName">Name of the font family to assign to this instance</param>
		public FontFamily (string familyName)
			: this (null, familyName)
		{
            this.Name = familyName;
		}

		/// <summary>
		/// Initializes a new instance of the FontFamily class with the given font <paramref name="familyName"/>
		/// </summary>
		/// <param name="generator">Generator to create this font family on</param>
		/// <param name="familyName">Name of the font family to assign to this instance</param>
		public FontFamily (Generator generator, string familyName)
			: base (generator, typeof(IFontFamily), true)
		{
            this.Name = familyName;

			Handler.Create (familyName);
		}

        private static IFontFamily CreateSystemFontFamilyHandler(
            string systemFontFamilyName, 
            Generator generator)
        {
            IFontFamily handler = null;

            var fonts = generator.CreateHandler<IFonts>();

            handler = fonts.GetSystemFontFamily(FontFamilies.SerifFamilyName);

            return handler;
        }

        /// <summary>
        /// If the family name is "serif", "sans-serif", "monospace",
        /// "cursive" or "fantasy", returns a font family with the
        /// same name whose handler uses a system-dependent font.
        /// </summary>
        /// <param name="familyName"></param>
        /// <param name="generator"></param>
        /// <returns></returns>
        public static FontFamily CreateWebFontFamily(
            string familyName, 
            Generator generator = null)
        {
            IFontFamily handler = null;

            var temp = generator ?? Generator.Current;

            switch (familyName.ToLowerInvariant())
            {
                case "serif":
                    handler = CreateSystemFontFamilyHandler(
                        FontFamilies.SerifFamilyName,
                        temp);
                    break;

                case "sans-serif":
                    handler = CreateSystemFontFamilyHandler(
                        FontFamilies.SansFamilyName,
                        temp);
                    break;

                case "monospace":
                    handler = CreateSystemFontFamilyHandler(
                        FontFamilies.MonospaceFamilyName,
                        temp);
                    break;
            }

            FontFamily result = null;

            if (handler != null)
                result =                     
                    new FontFamily(
                    generator, 
                    handler, 
                    // keep the original family name, e.g. sans, and
                    // allow the underlying handler to keep its own                    
                    familyName);
            else
                result =
                    new FontFamily(
                        generator ?? Generator.Current, 
                        familyName);

            return result;
        }

		/// <summary>
		/// Tests this instance for equality with another font family
		/// </summary>
		/// <remarks>
		/// Font families are considered equal if the names are the same
		/// </remarks>
		/// <param name="other">Other font to test</param>
		/// <returns>True if the families are equal, false otherwise</returns>
		public bool Equals (FontFamily other)
		{
			return other == this;
		}

		/// <summary>
		/// Tests two FontFamily objects for equality
		/// </summary>
		/// <remarks>
		/// Font families are considered equal if the names are the same
		/// </remarks>
		/// <param name="value1">First font family to test</param>
		/// <param name="value2">Second font family to test</param>
		/// <returns>True if the font families are equal, false otherwise</returns>
		public static bool operator == (FontFamily value1, FontFamily value2)
		{
			if (object.ReferenceEquals(value1, value2))
				return true;
			if (((object)value1) == null || ((object)value2) == null)
				return false;
			return value1.Name == value2.Name;
		}

		/// <summary>
		/// Tests two FontFamily objects for inequality
		/// </summary>
		/// <param name="value1">First font family to test</param>
		/// <param name="value2">Second font family to test</param>
		/// <returns>True if the font families are not equal, false otherwise</returns>
		public static bool operator != (FontFamily value1, FontFamily value2)
		{
			return !(value1 == value2);
		}

		/// <summary>
		/// Gets the hash code for this instance
		/// </summary>
		/// <returns>Hash code for this instance</returns>
		public override int GetHashCode ()
		{
			return Name.GetHashCode ();
		}

		/// <summary>
		/// Tests if this instance is equal to the specified object
		/// </summary>
		/// <param name="obj">Object to test with</param>
		/// <returns>True if the specified object is a FontFamily and is equal to this instance</returns>
		public override bool Equals (object obj)
		{
			return obj is FontFamily && this == (FontFamily)obj;
		}

		/// <summary>
		/// Gets a string representation of this font family
		/// </summary>
		/// <returns>String representation of this font family</returns>
		public override string ToString ()
		{
			return Name;
		}
	}
}

