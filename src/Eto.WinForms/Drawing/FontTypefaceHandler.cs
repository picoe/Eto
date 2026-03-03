using Eto.Shared.Drawing;
using sdt = System.Drawing.Text;

namespace Eto.WinForms.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<sd.FontStyle, FontTypeface>, FontTypeface.IHandler
	{
		string _name;
		string _postScriptName;
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
			_postScriptName = info?.PostScriptName;
			SetFontStyle(info.SubFamilyName);
		}
		
		public FontTypefaceHandler()
		{
		}

		public string Name => _name ?? (_name = GetName());
		
		public string PostScriptName
		{
			get
			{
				if (_postScriptName != null)
					return _postScriptName;
				// Try to read PostScript name from the font file via OpenTypeFontInfo
				_postScriptName = GetPostScriptNameFromOpenType();
				if (!string.IsNullOrWhiteSpace(_postScriptName))
					return _postScriptName;

				// Fallback: build a PostScript-like name from family/face
				var family = SDFontFamily.GetName(0);
				var face = Name;

				if (string.IsNullOrWhiteSpace(family))
					return null;

				family = family.Replace(" ", string.Empty);
				face = face?.Replace(" ", string.Empty);

				if (string.IsNullOrWhiteSpace(face) || string.Equals(face, "Regular", StringComparison.OrdinalIgnoreCase))
					return family;

				return $"{family}-{face}";
			}
		}
		
		string GetPostScriptNameFromOpenType()
		{
			try
			{
				var font = Font;
				var fontFilePath = Win32.GetFontFilePath(font);
				if (string.IsNullOrEmpty(fontFilePath))
					return null;

				var infos = OpenTypeFontInfo.FromFile(fontFilePath)
					.Where(r => r != null)
					.ToList();
				if (infos.Count == 0)
					return null;

				var familyName = SDFontFamily?.GetName(0) ?? SDFontFamily?.Name;
				if (!string.IsNullOrWhiteSpace(familyName))
				{
					var familyMatches = infos.Where(i =>
						string.Equals(i.TypographicFamilyName ?? i.FamilyName, familyName, StringComparison.OrdinalIgnoreCase)
						|| string.Equals(i.FamilyName, familyName, StringComparison.OrdinalIgnoreCase))
						.ToList();
					if (familyMatches.Count > 0)
						infos = familyMatches;
				}

				var faceName = Name;
				var match = infos.FirstOrDefault(i =>
					string.Equals(i.TypographicSubFamilyName ?? i.SubFamilyName, faceName, StringComparison.OrdinalIgnoreCase))
					?? infos.FirstOrDefault(i => MatchesFontStyle(i, Control))
					?? infos.FirstOrDefault();

				return match?.PostScriptName;
			}
			catch
			{
				return null;
			}
		}

		static bool MatchesFontStyle(OpenTypeFontInfo info, sd.FontStyle style)
		{
			var subFamily = info?.TypographicSubFamilyName ?? info?.SubFamilyName;
			if (string.IsNullOrWhiteSpace(subFamily))
				return false;

			var isBold = subFamily.IndexOf("bold", StringComparison.OrdinalIgnoreCase) >= 0;
			var isItalic = subFamily.IndexOf("italic", StringComparison.OrdinalIgnoreCase) >= 0
				|| subFamily.IndexOf("oblique", StringComparison.OrdinalIgnoreCase) >= 0;

			return isBold == style.HasFlag(sd.FontStyle.Bold)
				&& isItalic == style.HasFlag(sd.FontStyle.Italic);
		}


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
			_postScriptName = fontInfo?.PostScriptName;
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
			_postScriptName = fontInfo?.PostScriptName;
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
