using System;
using System.Collections.Generic;

namespace Eto.Drawing
{
	public interface IFontFamily : IInstanceWidget
	{
		string Name { get; }

		IEnumerable<FontTypeface> Typefaces { get; }

		void Create (string familyName);
	}

	public class FontFamily : InstanceWidget, IEquatable<FontFamily>
	{
		new IFontFamily Handler { get { return (IFontFamily)base.Handler; } }

		public string Name { get { return Handler.Name; } }

		public IEnumerable<FontTypeface> Typefaces { get { return Handler.Typefaces; } }

		[Obsolete("Use FontFamilies.Monospace")]
		public static readonly FontFamily Monospace = FontFamilies.Monospace;

		[Obsolete("Use FontFamilies.Sans")]
		public static readonly FontFamily Sans = FontFamilies.Sans;

		[Obsolete("Use FontFamilies.Serif")]
		public static readonly FontFamily Serif = FontFamilies.Serif;

		public FontFamily (Generator generator, IFontFamily handler)
			: base (generator, handler, true)
		{
		}

		public FontFamily (string familyName)
			: this (null, familyName)
		{
		}

		public FontFamily (Generator generator, string familyName)
			: base (generator, typeof(IFontFamily), true)
		{
			Handler.Create (familyName);
		}

		public bool Equals (FontFamily other)
		{
			return other == this;
		}

		public static bool operator == (FontFamily value1, FontFamily value2)
		{
			if (object.ReferenceEquals(value1, value2))
				return true;
			if (((object)value1) == null || ((object)value2) == null)
				return false;
			return value1.Name == value2.Name;
		}

		public static bool operator != (FontFamily value1, FontFamily value2)
		{
			return !(value1 == value2);
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			return obj is FontFamily && this == (FontFamily)obj;
		}

		public override string ToString ()
		{
			return Name;
		}
	}
}

