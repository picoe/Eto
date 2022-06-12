using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;
using System.Globalization;
using System.IO;

namespace Eto.Direct2D.Drawing
{
	/// <summary>
	/// Handler for <see cref="IFontFamily"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class FontFamilyHandler : WidgetHandler<sw.FontFamily, FontFamily>, FontFamily.IHandler
    {
		public string Name { get; private set; }

		public string LocalizedName
		{
			get
			{
				int index;
				if (!Control.FamilyNames.FindLocaleName(CultureInfo.CurrentUICulture.Name, out index))
					Control.FamilyNames.FindLocaleName("en-us", out index);
				return Control.FamilyNames.GetString(index);
			}
		}

		FontTypeface[] _typefaces;
		public IEnumerable<FontTypeface> Typefaces
		{
			get {
				return _typefaces ?? (_typefaces = Enumerable.Range(0, Control.FontCount)
					.Select(r => Control.GetFont(r))
					.Select(r => new FontTypeface(Widget, new FontTypefaceHandler(r)))
					.ToArray());
			} 
		}

		public FontFamilyHandler()
		{
		}

		public FontFamilyHandler(sw.FontFamily family)
		{
			Control = family;
			if (Control.FamilyNames.FindLocaleName("en-us", out var index))
				Name = Control.FamilyNames.GetLocaleName(index);
			else
				Name = Control.FamilyNames.GetString(0);
		}

		public void Create(string familyName)
		{
			string translatedName = Name = familyName;

			switch (familyName.ToUpperInvariant())
			{
				case FontFamilies.MonospaceFamilyName:
					translatedName = "Courier New";
					break;
				case FontFamilies.SansFamilyName:
					translatedName = "Microsoft Sans Serif";
					break;
				case FontFamilies.SerifFamilyName:
					translatedName = "Times New Roman";
					break;
				case FontFamilies.CursiveFamilyName:
					translatedName = "Comic Sans MS";
					break;
				case FontFamilies.FantasyFamilyName:
					translatedName = "Gabriola";
					break;
			}

			Control = FontHandler.GetFontFamily(translatedName);
		}

		public void CreateFromFiles(IEnumerable<string> fileNames)
		{
			throw new NotImplementedException();
		}

		public void CreateFromStreams(IEnumerable<Stream> streams)
		{
			throw new NotImplementedException();
		}
	}
}
