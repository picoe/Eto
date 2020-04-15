
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Drawing
{
	/// <summary>
	/// A font type that specifies the characteristics of a <see cref="FontFamily"/> variation
	/// </summary>
	/// <remarks>
	/// Each FontFamily can have different variations, such as Bold, Italic, Bold and Italic, etc.
	/// 
	/// This class represents each supported typeface of a particular font family, and can be used
	/// to create a <see cref="Font"/> instance that uses this typeface, using the <see cref="M:Font(FontTypeface,float,FontDecoration,Generator)"/> constructor.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class FontTypeface : Widget
	{
		new IHandler Handler => (IHandler)base.Handler;

		/// <summary>
		/// Gets the family of this typeface
		/// </summary>
		public FontFamily Family { get; private set; }

		/// <summary>
		/// Gets the name of this typeface
		/// </summary>
		/// <remarks>
		/// The name of the typeface typically includes hints to the style of the type
		/// </remarks>
		public string Name => Handler.Name;

		/// <summary>
		/// Gets the localized name of this typeface
		/// </summary>
		/// <remarks>
		/// This will return a name suitable to display to the user in their own language, if the font provides a specific name for their UI language.
		/// 
		/// For platforms that do not support localized font names, or for fonts that do not have a specific name for the current language, the
		/// value of <see cref="Name"/> will be returned.
		/// </remarks>
		public string LocalizedName => Handler.LocalizedName;

		/// <summary>
		/// Gets the style of this typeface
		/// </summary>
		/// <remarks>
		/// This style does not fully describe the characteristics of the typeface, just very broad characteristics.
		/// </remarks>
		public FontStyle FontStyle => Handler.FontStyle;

		/// <summary>
		/// Gets a value indicating that this font typeface has a bold style
		/// </summary>
		public bool Bold => FontStyle.HasFlag(FontStyle.Bold);

		/// <summary>
		/// Gets a value indicating that this font typeface has an italic style
		/// </summary>
		public bool Italic => FontStyle.HasFlag(FontStyle.Italic);

		/// <summary>
		/// Initializes a new instance of a FontTypeface class with the specified handler
		/// </summary>
		/// <remarks>
		/// This allows the platform handlers to create instances of the FontTypeface class
		/// with a specific handler. It should not be called by user code.
		/// </remarks>
		/// <param name="family">Family this typeface is part of</param>
		/// <param name="handler">Handler to use for this typeface instance</param>
		public FontTypeface(FontFamily family, IHandler handler)
			: base(handler)
		{
			Family = family;
		}

		/// <summary>
		/// Gets a string representation of this typeface
		/// </summary>
		/// <returns>A string representation of this typeface</returns>
		public override string ToString() => Name;

		/// <summary>
		/// Tests this instance for equality with another font typeface
		/// </summary>
		/// <remarks>
		/// Font typefaces are considered equal if the names are the same
		/// </remarks>
		/// <param name="other">Other font typeface to test</param>
		/// <returns>True if the typefaces are equal, false otherwise</returns>
		public bool Equals(FontTypeface other) => other == this;

		/// <summary>
		/// Tests two FontTypeface objects for equality
		/// </summary>
		/// <remarks>
		/// Font typefaces are considered equal if the names and font typefaces are the same
		/// </remarks>
		/// <param name="value1">First font typeface to test</param>
		/// <param name="value2">Second font typeface to test</param>
		/// <returns>True if the font families are equal, false otherwise</returns>
		public static bool operator ==(FontTypeface value1, FontTypeface value2)
		{
			if (ReferenceEquals(value1, value2))
				return true;
			if (ReferenceEquals(value1, null) || ReferenceEquals(value2, null))
				return false;
			return value1.Name == value2.Name;
		}

		/// <summary>
		/// Tests two FontTypeface objects for inequality
		/// </summary>
		/// <param name="value1">First font typeface to test</param>
		/// <param name="value2">Second font typeface to test</param>
		/// <returns>True if the font typefaces are not equal, false otherwise</returns>
		public static bool operator !=(FontTypeface value1, FontTypeface value2) => !(value1 == value2);

		/// <summary>
		/// Gets the hash code for this instance
		/// </summary>
		/// <returns>Hash code for this instance</returns>
		public override int GetHashCode()
		{
			var hash = 23;
			hash = hash * 31 + Family.GetHashCode();
			hash = hash * 31 & Name.GetHashCode();
			return hash;
		}

		/// <summary>
		/// Tests if this instance is equal to the specified object
		/// </summary>
		/// <param name="obj">Object to test with</param>
		/// <returns>True if the specified object is a FontTypeface and is equal to this instance</returns>
		public override bool Equals(object obj) => this == obj as FontTypeface;

		/// <summary>
		/// Gets a value indicating that this font is a symbol font and not generally used for text
		/// </summary>
		/// <remarks>
		/// Some platforms (e.g. Gtk) might not support this and simply return false for all fonts.
		/// </remarks>
		public bool IsSymbol => Handler.IsSymbol;

		/// <summary>
		/// Gets a value indicating that this font supports the character range specified
		/// </summary>
		/// <param name="start">Start of the range</param>
		/// <param name="end">End of the range (inclusive)</param>
		/// <returns>True if the font supports the characters in the specified range, false otherwise</returns>
		public bool HasCharacterRange(int start, int end) => Handler.HasCharacterRanges(new[] { new Range<int>(start, end) });

		/// <summary>
		/// Gets a value indicating that this font supports the character range specified
		/// </summary>
		/// <param name="range">Range to test</param>
		/// <returns>True if the font supports the characters in the specified range, false otherwise</returns>
		public bool HasCharacterRange(Range<int> range) => Handler.HasCharacterRanges(new[] { range });

		/// <summary>
		/// Gets a value indicating that this font supports the character ranges specified
		/// </summary>
		/// <param name="ranges">Ranges to test</param>
		/// <returns>True if the font supports all characters in the specified ranges, false otherwise</returns>
		public bool HasCharacterRanges(IEnumerable<Range<int>> ranges) => Handler.HasCharacterRanges(ranges);


		/// <summary>
		/// Platform handler interface for the <see cref="FontTypeface"/> class
		/// </summary>
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Gets the name of this typeface
			/// </summary>
			/// <remarks>
			/// The name of the typeface typically includes hints to the style of the type
			/// </remarks>
			string Name { get; }

			/// <summary>
			/// Gets the localized name of this typeface
			/// </summary>
			string LocalizedName { get; }

			/// <summary>
			/// Gets the style of this typeface
			/// </summary>
			/// <remarks>
			/// This style does not fully describe the characteristics of the typeface, just very broad characteristics.
			/// </remarks>
			FontStyle FontStyle { get; }

			/// <summary>
			/// Gets a value indicating that this font is a symbol font and not generally used for text
			/// </summary>
			bool IsSymbol { get; }

			/// <summary>
			/// Gets a value indicating that this font supports the character ranges specified
			/// </summary>
			/// <param name="ranges">Ranges to test</param>
			/// <returns>True if the font supports all characters in the specified ranges, false otherwise</returns>
			bool HasCharacterRanges(IEnumerable<Range<int>> ranges);
		}
	}
}
