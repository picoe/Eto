using System.Windows;
using Microsoft.Win32;
using System.Linq;
using System.Text;
#if WPF
using Eto.Wpf.Forms;
#elif WINFORMS
using Eto.WinForms.Forms;
#endif

namespace Eto
{
	static partial class Win32
	{
		public static TEXTMETRICW GetTextMetrics(this sd.Font font)
		{
			using (var graphics = new swf.Control().CreateGraphics())
			{
				var hDC = graphics.GetHdc();

				var hFont = font.ToHfont();
				var hFontDefault = SelectObject(hDC, hFont);

				GetTextMetrics(hDC, out var textMetric);
				return textMetric;
			}
		}

		public static OUTLINETEXTMETRICW GetOutlineTextMetrics(this sd.Font font)
		{
			var graphics = sd.Graphics.FromHwnd(IntPtr.Zero);
			var hdc = IntPtr.Zero;
			var hFont = IntPtr.Zero;
			var old = IntPtr.Zero;
			var outlineTextMetric = IntPtr.Zero;
			try
			{
				hdc = graphics.GetHdc();
				hFont = font.ToHfont();
				old = SelectObject(hdc, hFont);

				var size = GetOutlineTextMetrics(hdc, 0, IntPtr.Zero);
				if (size == 0)
					return default;

				outlineTextMetric = Marshal.AllocHGlobal((int)size);
				if (GetOutlineTextMetrics(hdc, size, outlineTextMetric) == 0)
					return default;

				return Marshal.PtrToStructure<OUTLINETEXTMETRICW>(outlineTextMetric);
			}
			finally
			{
				if (outlineTextMetric != IntPtr.Zero)
					Marshal.FreeHGlobal(outlineTextMetric);
				if (old != IntPtr.Zero)
					SelectObject(hdc, old);
				if (hFont != IntPtr.Zero)
					DeleteObject(hFont);
				if (hdc != IntPtr.Zero)
					graphics.ReleaseHdc(hdc);
				graphics.Dispose();
			}
		}

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		public static extern bool GetTextMetrics(IntPtr hdc, out TEXTMETRICW lptm);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
		public static extern uint GetOutlineTextMetrics(IntPtr hdc, uint cbData, IntPtr lpOTM);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct TEXTMETRICW
		{
			public int tmHeight;
			public int tmAscent;
			public int tmDescent;
			public int tmInternalLeading;
			public int tmExternalLeading;
			public int tmAveCharWidth;
			public int tmMaxCharWidth;
			public int tmWeight;
			public int tmOverhang;
			public int tmDigitizedAspectX;
			public int tmDigitizedAspectY;
			public ushort tmFirstChar;
			public ushort tmLastChar;
			public ushort tmDefaultChar;
			public ushort tmBreakChar;
			public byte tmItalic;
			public byte tmUnderlined;
			public byte tmStruckOut;
			public byte tmPitchAndFamily;
			public byte tmCharSet;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct PANOSE
		{
			public byte bFamilyType;
			public byte bSerifStyle;
			public byte bWeight;
			public byte bProportion;
			public byte bContrast;
			public byte bStrokeVariation;
			public byte bArmStyle;
			public byte bLetterform;
			public byte bMidline;
			public byte bXHeight;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct OUTLINETEXTMETRICW
		{
			public uint otmSize;
			public TEXTMETRICW otmTextMetrics;
			public byte otmFiller;
			public PANOSE otmPanoseNumber;
			public uint otmfsSelection;
			public uint otmfsType;
			public int otmsCharSlopeRise;
			public int otmsCharSlopeRun;
			public int otmItalicAngle;
			public uint otmEMSquare;
			public int otmAscent;
			public int otmDescent;
			public uint otmLineGap;
			public uint otmsCapEmHeight;
			public uint otmsXHeight;
			public RECT otmrcFontBox;
			public int otmMacAscent;
			public int otmMacDescent;
			public uint otmMacLineGap;
			public uint otmusMinimumPPEM;
			public POINT otmptSubscriptSize;
			public POINT otmptSubscriptOffset;
			public POINT otmptSuperscriptSize;
			public POINT otmptSuperscriptOffset;
			public uint otmsStrikeoutSize;
			public int otmsStrikeoutPosition;
			public int otmsUnderscoreSize;
			public int otmsUnderscorePosition;
			public IntPtr otmpFamilyName;
			public IntPtr otmpFaceName;
			public IntPtr otmpStyleName;
			public IntPtr otmpFullName;
		}

		[DllImport("gdi32.dll")]
		public static extern uint GetFontUnicodeRanges(IntPtr hdc, IntPtr lpgs);

		[DllImport("gdi32.dll")]
		public extern static IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

		public struct FontRange
		{
			public UInt16 Low;
			public UInt16 High;
		}

		public static List<FontRange> GetUnicodeRangesForFont(this sd.Font font)
		{
			var g = sd.Graphics.FromHwnd(IntPtr.Zero);
			IntPtr hdc = g.GetHdc();
			IntPtr hFont = font.ToHfont();
			IntPtr old = SelectObject(hdc, hFont);
			uint size = GetFontUnicodeRanges(hdc, IntPtr.Zero);
			IntPtr glyphSet = Marshal.AllocHGlobal((int)size);
			GetFontUnicodeRanges(hdc, glyphSet);
			List<FontRange> fontRanges = new List<FontRange>();
			int count = Marshal.ReadInt32(glyphSet, 12);
			for (int i = 0; i < count; i++)
			{
				FontRange range = new FontRange();
				range.Low = (UInt16)Marshal.ReadInt16(glyphSet, 16 + i * 4);
				range.High = (UInt16)(range.Low + Marshal.ReadInt16(glyphSet, 18 + i * 4) - 1);
				fontRanges.Add(range);
			}
			SelectObject(hdc, old);
			Marshal.FreeHGlobal(glyphSet);
			g.ReleaseHdc(hdc);
			g.Dispose();
			return fontRanges;
		}
		
		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
		static extern int GetTextFace(IntPtr hdc, int nCount, StringBuilder lpFaceName);

		static string GetFaceName(sd.Font font)
		{
			var graphics = sd.Graphics.FromHwnd(IntPtr.Zero);
			var hdc = graphics.GetHdc();
			var hFont = IntPtr.Zero;
			var old = IntPtr.Zero;
			try
			{
				hFont = font.ToHfont();
				old = SelectObject(hdc, hFont);

				var sb = new StringBuilder(64);
				if (GetTextFace(hdc, sb.Capacity, sb) > 0)
					return sb.ToString();

				return font.FontFamily?.Name;
			}
			finally
			{
				if (old != IntPtr.Zero)
					SelectObject(hdc, old);
				if (hFont != IntPtr.Zero)
					DeleteObject(hFont);
				graphics.ReleaseHdc(hdc);
				graphics.Dispose();
			}
		}

		static string ResolveFontPath(string fileName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
				return null;

			if (Path.IsPathRooted(fileName))
				return File.Exists(fileName) ? fileName : null;

			var fontsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
			var fullPath = Path.Combine(fontsFolder, fileName);
			return File.Exists(fullPath) ? fullPath : null;
		}

		static int GetFontNameMatchScore(string registryName, string faceName, bool bold, bool italic)
		{
			if (string.IsNullOrWhiteSpace(registryName) || string.IsNullOrWhiteSpace(faceName))
				return -1;

			var normalizedRegistryName = registryName;
			const string trueTypeSuffix = "(TrueType)";
			var suffixIndex = normalizedRegistryName.IndexOf(trueTypeSuffix, StringComparison.OrdinalIgnoreCase);
			if (suffixIndex >= 0)
				normalizedRegistryName = normalizedRegistryName.Remove(suffixIndex, trueTypeSuffix.Length);
			normalizedRegistryName = normalizedRegistryName.Trim();
			var normalizedFaceName = faceName.Trim();

			int score;
			if (normalizedRegistryName.StartsWith(normalizedFaceName, StringComparison.OrdinalIgnoreCase))
				score = 100;
			else if (normalizedRegistryName.IndexOf(normalizedFaceName, StringComparison.OrdinalIgnoreCase) >= 0)
				score = 70;
			else
				return -1;

			var regBold = normalizedRegistryName.IndexOf("bold", StringComparison.OrdinalIgnoreCase) >= 0;
			var regItalic = normalizedRegistryName.IndexOf("italic", StringComparison.OrdinalIgnoreCase) >= 0
				|| normalizedRegistryName.IndexOf("oblique", StringComparison.OrdinalIgnoreCase) >= 0;

			score += regBold == bold ? 20 : -10;
			score += regItalic == italic ? 20 : -10;
			return score;
		}

		static void AddMatchesFromRegistry(RegistryKey key, string faceName, bool bold, bool italic, List<(string path, int score)> matches)
		{
			if (key == null)
				return;

			foreach (var valueName in key.GetValueNames())
			{
				if (string.IsNullOrWhiteSpace(valueName))
					continue;

				var score = GetFontNameMatchScore(valueName, faceName, bold, italic);
				if (score < 0)
					continue;

				var value = key.GetValue(valueName) as string;
				var path = ResolveFontPath(value);
				if (!string.IsNullOrWhiteSpace(path))
					matches.Add((path, score));
			}
		}

		public static string GetFontFilePath(sd.Font font)
		{
			if (font == null)
				return null;

			try
			{
				var faceName = GetFaceName(font);
				if (string.IsNullOrWhiteSpace(faceName))
					return null;

				var bold = font.Style.HasFlag(sd.FontStyle.Bold);
				var italic = font.Style.HasFlag(sd.FontStyle.Italic);
				var matches = new List<(string path, int score)>();

				using (var machineKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Fonts"))
				{
					AddMatchesFromRegistry(machineKey, faceName, bold, italic, matches);
				}
				using (var userKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Fonts"))
				{
					AddMatchesFromRegistry(userKey, faceName, bold, italic, matches);
				}

				return matches
					.OrderByDescending(r => r.score)
					.Select(r => r.path)
					.FirstOrDefault();
			}
			catch
			{
				return null;
			}
		}
	}
}
