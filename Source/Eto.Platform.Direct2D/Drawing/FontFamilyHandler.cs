using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;

namespace Eto.Platform.Direct2D.Drawing
{
	/// <summary>
	/// Handler for <see cref="IFontFamily"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class FontFamilyHandler : WidgetHandler<FontFamily>, IFontFamily
    {
		public string Name { get; private set; }
		public string TranslatedName { get; private set; }

		public IEnumerable<FontTypeface> Typefaces
		{
			get { return new List<FontTypeface>(); } // TODO
		}

		public FontFamilyHandler()
		{
		}

		public FontFamilyHandler(string name)
		{
			Name = TranslatedName = name;
		}

		public void Create(string familyName)
		{
			this.Name = familyName;

			switch (familyName.ToUpperInvariant())
			{
				case FontFamilies.MonospaceFamilyName:
					TranslatedName = "Courier New";
					break;
				case FontFamilies.SansFamilyName:
					TranslatedName = "Microsoft Sans Serif";
					break;
				case FontFamilies.SerifFamilyName:
					TranslatedName = "Times New Roman";
					break;
				case FontFamilies.CursiveFamilyName:
					TranslatedName = "Comic Sans MS";
					break;
				case FontFamilies.FantasyFamilyName:
					TranslatedName = "Gabriola";
					break;
				default:
					TranslatedName = familyName;
					break;
			}
		}

		public string ID { get; set; }

		public object ControlObject
		{
			get { return Name; }
		}

		public void HandleEvent(string id, bool defaultEvent = false)
		{
		}
	}
}
