using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using sd = System.Drawing;

namespace Eto.WinForms.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<sd.FontStyle, FontTypeface>, FontTypeface.IHandler
	{
		string name;
		bool? _isSymbol;
		sd.Font _font;
		List<Win32.FontRange> _fontRanges;
		FontFamilyHandler FamilyHandler => (FontFamilyHandler)Widget.Family.Handler;

		public FontTypefaceHandler(sd.FontStyle style)
		{
			Control = style;
		}

		public string Name => name ?? (name = GetName());

		public string LocalizedName => Name;

		public FontStyle FontStyle => Control.ToEtoStyle();

		public bool IsSymbol => _isSymbol ?? (_isSymbol = GetIsSymbol()).Value;

		bool GetIsSymbol()
		{
			var metrics = Font.GetTextMetrics();
			return metrics.tmCharSet == 2;
		}

		sd.Font Font => _font ?? (_font = GetFont());
		sd.Font GetFont() => new sd.Font(FamilyHandler.Control, 1, Control);

		List<Win32.FontRange> FontRanges => _fontRanges ?? (_fontRanges = Font.GetUnicodeRangesForFont());

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

		string GetName() => FontStyle.ToString().Replace(",", string.Empty);
	}
}
