using Eto.Drawing;
using Eto.Forms;
using Eto.Shared.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using sd = System.Drawing;
using sdt = System.Drawing.Text;

namespace Eto.WinForms.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<sd.FontStyle, FontTypeface>, FontTypeface.IHandler
	{
		string _name;
		bool? _isSymbol;
		sd.Font _font;
		List<Win32.FontRange> _fontRanges;
		sd.FontFamily _sdfamily;
		public FontFamilyHandler FamilyHandler => (FontFamilyHandler)Widget.Family.Handler;

		public FontTypefaceHandler(sd.FontFamily sdfamily, sd.FontStyle style)
		{
			_sdfamily = sdfamily;
			Control = style;
			if (FontsHandler.UseTypographicFonts)
			{
				_name = FontsHandler.FindFontTypefaceName(_sdfamily, Control);
			}
		}

		internal FontTypefaceHandler(sd.FontFamily sdfamily, OpenTypeFontInfo info, string variationName = null)
		{
			_sdfamily = sdfamily;
			_name = variationName ?? info.TypographicSubFamilyName ?? info.SubFamilyName;
			SetFontStyle(info.SubFamilyName);
		}
		
		public FontTypefaceHandler()
		{
		}

		public string Name => _name ?? (_name = GetName());

		public string LocalizedName => Name;

		public FontStyle FontStyle => Control.ToEtoStyle();

		public bool IsSymbol => _isSymbol ?? (_isSymbol = GetIsSymbol()).Value;

		bool GetIsSymbol()
		{
			var metrics = Font.GetTextMetrics();
			return metrics.tmCharSet == 2;
		}

		sd.Font Font => _font ?? (_font = GetFont());
		sd.Font GetFont() => new sd.Font(SDFontFamily, 1, Control);

		public sd.FontFamily SDFontFamily => _sdfamily ?? FamilyHandler.Control;

		List<Win32.FontRange> FontRanges => _fontRanges ?? (_fontRanges = Font.GetUnicodeRangesForFont());

		public FontFamily Family { get; private set; }

		public bool HasCharacterRanges(IEnumerable<Range<int>> ranges)
		{
			var supportedRanges = FontRanges;

			foreach (var range in ranges)
			{
				for (int i = range.Start; i <= range.End; i++)
				{
					UInt16 intval = Convert.ToUInt16(i);
					bool isCharacterPresent = false;
					foreach (var supportedRange in supportedRanges)
					{
						if (intval >= supportedRange.Low && intval <= supportedRange.High)
						{
							isCharacterPresent = true;
							break;
						}
					}
					if (!isCharacterPresent)
						return false;
				}
			}

			return true;
		}

		string GetName() => Control.ToString().Replace(",", string.Empty);
		
		void SetFontStyle(string subFamilyName)
		{
			if (subFamilyName == null)
				return;
				
			if (subFamilyName.IndexOf("italic", StringComparison.OrdinalIgnoreCase) >= 0)
				Control |= sd.FontStyle.Italic;
			if (subFamilyName.IndexOf("bold", StringComparison.OrdinalIgnoreCase) >= 0)
				Control |= sd.FontStyle.Bold;
		}

		public unsafe void Create(Stream stream)
		{
			var fontCollection = new sdt.PrivateFontCollection();
			OpenTypeFontInfo fontInfo = null;
			using (var ms = new MemoryStream())
			{
				stream.CopyTo(ms);

				var bytes = ms.ToArray();
				fixed (byte* ptr = bytes)
				{
					fontCollection.AddMemoryFont((IntPtr)ptr, bytes.Length);
				}
				ms.Position = 0;
				fontInfo = OpenTypeFontInfo.FromStream(ms).Single();
			}
		
			var families = fontCollection.Families;
			
			if (families.Length == 0)
				throw new ArgumentOutOfRangeException(nameof(stream), "Could not load font from stream");


			_name = fontInfo?.TypographicSubFamilyName ?? fontInfo?.SubFamilyName;
			SetFontStyle(fontInfo?.SubFamilyName);

			var sdfamily = families[0];
			var familyHandler = new FontFamilyHandler(sdfamily);
			familyHandler.SetFontCollection(fontCollection);
			familyHandler.SetTypefaces(new[] { Widget });
			Family = new FontFamily(familyHandler);
			
		}

		public void Create(string fileName)
		{
			var fontCollection = new sdt.PrivateFontCollection();
			fontCollection.AddFontFile(fileName);
			var families = fontCollection.Families;
			
			if (families.Length == 0)
				throw new ArgumentOutOfRangeException(nameof(fileName), "Could not load font from file");


			var fontInfo = OpenTypeFontInfo.FromFile(fileName).Single();
			_name = fontInfo?.TypographicSubFamilyName ?? fontInfo?.SubFamilyName;
			SetFontStyle(fontInfo?.SubFamilyName);

			var sdfamily = families[0];
			var familyHandler = new FontFamilyHandler(sdfamily);
			familyHandler.SetFontCollection(fontCollection);
			familyHandler.SetTypefaces(new[] { Widget });
			Family = new FontFamily(familyHandler);
		}
		
		public void Create(FontFamily family)
		{
			Family = family;
		}
	}
}
