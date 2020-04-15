using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Drawing;
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

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		public static extern bool GetTextMetrics(IntPtr hdc, out TEXTMETRICW lptm);

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
	}
}
