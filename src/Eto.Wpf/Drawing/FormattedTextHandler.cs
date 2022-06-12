using Eto.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sw = System.Windows;
using swm = System.Windows.Media;
using swmt = System.Windows.Media.TextFormatting;

namespace Eto.Wpf.Drawing
{
	public class FormattedTextHandler : WidgetHandler<swm.FormattedText, FormattedText, FormattedText.ICallback>, FormattedText.IHandler
	{
		string _text;
		SizeF _maxSize = SizeF.MaxValue;
		FormattedTextWrapMode _wrap;
		Font _font = SystemFonts.Default();
		FormattedTextAlignment _alignment;
		FormattedTextTrimming _trimming;
		Brush _foregroundBrush;
		bool _hasNewLines;
		bool _shouldClip;
		public FormattedTextWrapMode Wrap
		{
			get => _wrap;
			set
			{
				_wrap = value;
				Invalidate();
			}
		}
		public FormattedTextTrimming Trimming
		{
			get => _trimming;
			set
			{
				_trimming = value;
				if (Wrap == FormattedTextWrapMode.Character)
					Invalidate();
				else if (HasControl)
					SetTrimming(Control);
			}
		}
		public string Text
		{
			get => _text;
			set
			{
				_text = value;
				Invalidate();
			}
		}
		public SizeF MaximumSize
		{
			get => _maxSize;
			set
			{
				_maxSize = value;
				if (HasControl)
					SetMaxSize(Control);
			}
		}
		public Font Font
		{
			get => _font;
			set
			{
				_font = value;
				if (HasControl)
				{
					SetFont(Control);
					SetMaxSize(Control);
				}
			}
		}
		public Brush ForegroundBrush
		{
			get => _foregroundBrush;
			set
			{
				_foregroundBrush = value;
				if (HasControl)
					SetForegroundBrush(Control);
			}
		}
		public FormattedTextAlignment Alignment
		{
			get => _alignment;
			set
			{
				_alignment = value;
				if (HasControl)
				{
					SetTextAlignment(Control);
					SetMaxSize(Control);
				}
			}
		}

		public SizeF Measure()
		{
			var control = Control;
			double width = control.WidthIncludingTrailingWhitespace;
			if (_shouldClip)
				width = Math.Min(width, MaximumSize.Width);

			return new SizeF((float)width, (float)control.Height);
		}

		void Invalidate()
		{
			Control = null;
		}

		protected override swm.FormattedText CreateControl()
		{
			var font = Font;
			var text = Text ?? string.Empty;
			text = SetWrap(text);
			_hasNewLines = text.IndexOf('\n') != -1;

#pragma warning disable CS0618 // 'FormattedText.FormattedText(string, CultureInfo, FlowDirection, Typeface, double, Brush)' is obsolete: 'Use the PixelsPerDip override'
			var formattedText = new swm.FormattedText(
				text,
				CultureInfo.CurrentUICulture,
				sw.FlowDirection.LeftToRight,
				font.ToWpfTypeface(),
				font.Size,
				ForegroundBrush.ToWpf());
#pragma warning restore CS0618 // 'FormattedText.FormattedText(string, CultureInfo, FlowDirection, Typeface, double, Brush)' is obsolete: 'Use the PixelsPerDip override'

			// support correctly showing ellipsis when there's a single line
			if (Wrap == FormattedTextWrapMode.None)
			{
				if (!_hasNewLines)
					formattedText.MaxLineCount = 1;
				else
				{
					// can't show ellipsis when there's multiple lines currently..
					formattedText.MaxLineCount = int.MaxValue;
					_shouldClip = true;
				}
			}
			SetTextAlignment(formattedText);
			SetFont(formattedText);
			SetTrimming(formattedText);
			SetMaxSize(formattedText);
			return formattedText;
		}

		string SetWrap(string text)
		{
			// character wrap only works with no trimming.
			if ((Wrap == FormattedTextWrapMode.Character || Wrap == FormattedTextWrapMode.None) && Trimming == FormattedTextTrimming.None)
			{
				// wpf will always word wrap, so we replace spaces with nbsp
				// so that it is forced to wrap at the character level
				text = text.Replace(' ', (char)0xa0); // no break space
			}
			return text;
		}

		void SetTextAlignment(swm.FormattedText formattedText)
		{
			switch (Alignment)
			{
				case FormattedTextAlignment.Left:
					formattedText.TextAlignment = sw.TextAlignment.Left;
					break;
				case FormattedTextAlignment.Right:
					formattedText.TextAlignment = sw.TextAlignment.Right;
					break;
				case FormattedTextAlignment.Center:
					formattedText.TextAlignment = sw.TextAlignment.Center;
					break;
				case FormattedTextAlignment.Justify:
					formattedText.TextAlignment = sw.TextAlignment.Justify;
					break;
				default:
					break;
			}

		}

		void SetFont(swm.FormattedText formattedText)
		{
			formattedText.SetEtoFont(Font);
		}

		void SetMaxSize(swm.FormattedText formattedText)
		{
			if ((Alignment == FormattedTextAlignment.Left || MaximumSize.Width < float.MaxValue) && (Wrap != FormattedTextWrapMode.None || !_hasNewLines))
				formattedText.MaxTextWidth = Math.Min(3579139, MaximumSize.Width);
			else
				formattedText.MaxTextWidth = formattedText.WidthIncludingTrailingWhitespace;
			formattedText.MaxTextHeight = Math.Min(3579139, MaximumSize.Height);
		}
		void SetTrimming(swm.FormattedText formattedText)
		{
			switch (Trimming)
			{
				case FormattedTextTrimming.None:
					formattedText.Trimming = sw.TextTrimming.None;
					break;
				case FormattedTextTrimming.CharacterEllipsis:
					formattedText.Trimming = sw.TextTrimming.CharacterEllipsis;
					break;
				case FormattedTextTrimming.WordEllipsis:
					formattedText.Trimming = sw.TextTrimming.WordEllipsis;
					break;
				default:
					break;
			}
		}

		void SetForegroundBrush(swm.FormattedText formattedText)
		{
			formattedText.SetForegroundBrush(ForegroundBrush.ToWpf());
		}

		public void DrawText(GraphicsHandler handler, PointF location)
		{
			/**
			//Doesn't do font fallbacks, so it isn't very useful at this point.
			//Only other way appears to re-write the FormattedText class which comes along with a TON of code.
			// if we can get font fallbacks working here, this might be a feasible way to support (fast) character wrapping
			if (Wrap == FormattedTextWrapMode.Character)
			{
				var glyphRun = ConvertTextLinesToGlyphRun(Font.ToWpfTypeface(), Font.Size * 96.0 / 72.0, location.ToWpf(), Text);
				if (glyphRun != null)
					handler.Control.DrawGlyphRun(Brushes.Red.ToWpf(), glyphRun);
			}
			/**/
			if (_shouldClip)
			{
				// a better way here would be to draw each line separately so alignment works on a per-paragraph basis
				// but this is an edge case we don't fully support yet.
				var rect = new sw.Rect(location.X, location.Y, Math.Min(MaximumSize.Width, Control.WidthIncludingTrailingWhitespace), Control.Height);
				handler.Control.PushClip(new swm.RectangleGeometry(rect));
				handler.Control.DrawText(Control, location.ToWpf());
				handler.Control.Pop();
			}
			else
				handler.Control.DrawText(Control, location.ToWpf());
		}


		swm.GlyphRun ConvertTextLinesToGlyphRun(swm.Typeface typeface, double fontSize, sw.Point location, string text)
		{
			var glyphIndices = new List<ushort>();
			var advanceWidths = new List<double>();
			var glyphOffsets = new List<sw.Point>();

			if (!typeface.TryGetGlyphTypeface(out var glyphTypeface))
				return null;

			location.Y += glyphTypeface.Baseline * fontSize;
			var y = 0.0;
			var line = text;
			var lineHeight = glyphTypeface.Height * fontSize;
			var x = 0.0;

			for (int i = 0; i < line.Length; ++i)
			{
				if (!glyphTypeface.CharacterToGlyphMap.TryGetValue(line[i], out var glyphIndex))
					continue;

				var advanceWidth = glyphTypeface.AdvanceWidths[glyphIndex] * fontSize;
				if (x + advanceWidth > _maxSize.Width)
				{
					y -= lineHeight;
					if ((-y) + lineHeight >= _maxSize.Height)
						break;
					x = 0;
				}

				glyphIndices.Add(glyphIndex);
				advanceWidths.Add(0);
				glyphOffsets.Add(new sw.Point(x, y));

				x += advanceWidth;
			}

#pragma warning disable CS0618 // 'GlyphRun.GlyphRun(GlyphTypeface, int, bool, double, IList<ushort>, Point, IList<double>, IList<Point>, IList<char>, string, IList<ushort>, IList<bool>, XmlLanguage)' is obsolete: 'Use the PixelsPerDip override'
			return new swm.GlyphRun(
				glyphTypeface,
				0,
				false,
				fontSize,
				glyphIndices,
				location,
				advanceWidths,
				glyphOffsets,
				null,
				null,
				null,
				null,
				null);
#pragma warning restore CS0618 // 'GlyphRun.GlyphRun(GlyphTypeface, int, bool, double, IList<ushort>, Point, IList<double>, IList<Point>, IList<char>, string, IList<ushort>, IList<bool>, XmlLanguage)' is obsolete: 'Use the PixelsPerDip override'
		}
	}
}

