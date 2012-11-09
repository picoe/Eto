using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Drawing
{
	public interface IFontTypeface : IInstanceWidget
	{
		string Name { get; }

		FontStyle FontStyle { get; }
	}

	public class FontTypeface : InstanceWidget
	{
		new IFontTypeface Handler  { get { return (IFontTypeface)base.Handler; } }

		/// <summary>
		/// Gets the family of this typeface
		/// </summary>
		public FontFamily Family { get; private set; }

		/// <summary>
		/// Gets the name of this typeface
		/// </summary>
		public string Name { get { return Handler.Name; } }

		/// <summary>
		/// Gets the style of this typeface
		/// </summary>
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
		public FontTypeface (FontFamily family, IFontTypeface handler)
			: base(family.Generator, handler)
		{
			this.Family = family;
		}

		public override string ToString ()
		{
			return Name;
		}
	}
}
