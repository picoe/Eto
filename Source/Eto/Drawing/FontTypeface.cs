
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
	public class FontTypeface : Widget
	{
		new IHandler Handler  { get { return (IHandler)base.Handler; } }

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
		public string Name { get { return Handler.Name; } }

		/// <summary>
		/// Gets the style of this typeface
		/// </summary>
		/// <remarks>
		/// This style does not fully describe the characteristics of the typeface, just very broad characteristics.
		/// </remarks>
		public FontStyle FontStyle { get { return Handler.FontStyle; } }

		/// <summary>
		/// Gets a value indicating that this font typeface has a bold style
		/// </summary>
		public bool Bold
		{
			get { return FontStyle.HasFlag (FontStyle.Bold); }
		}

		/// <summary>
		/// Gets a value indicating that this font typeface has an italic style
		/// </summary>
		public bool Italic
		{
			get { return FontStyle.HasFlag (FontStyle.Italic); }
		}

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
			this.Family = family;
		}

		/// <summary>
		/// Gets a string representation of this typeface
		/// </summary>
		/// <returns>A string representation of this typeface</returns>
		public override string ToString ()
		{
			return Name;
		}

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
			/// Gets the style of this typeface
			/// </summary>
			/// <remarks>
			/// This style does not fully describe the characteristics of the typeface, just very broad characteristics.
			/// </remarks>
			FontStyle FontStyle { get; }
		}
	}
}
