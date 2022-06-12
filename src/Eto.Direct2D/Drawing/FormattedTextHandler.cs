using System;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;

namespace Eto.Direct2D.Drawing
{
	public class FormattedTextHandler : WidgetHandler<sw.TextLayout, FormattedText, FormattedText.ICallback>, FormattedText.IHandler
	{
		FormattedTextTrimming _trimming;
		FormattedTextAlignment _alignment;
		FormattedTextWrapMode _wrap;
		SizeF _maximumSize = SizeF.MaxValue;
		string _text;
		Font _font = SystemFonts.Default();

		public FormattedTextWrapMode Wrap
		{
			get => _wrap;
			set
			{
				if (_wrap != value)
				{
					_wrap = value;
					if (HasControl)
						SetWrap(Control);
				}
			}
		}
		public FormattedTextTrimming Trimming
		{
			get => _trimming;
			set
			{
				if (_trimming != value)
				{
					_trimming = value;
					if (HasControl)
						SetTrimming(Control);
				}
			}
		}

		private void SetTrimming(sw.TextLayout layout)
		{
			switch (_trimming)
			{
				case FormattedTextTrimming.None:
					layout.SetTrimming(new sw.Trimming { Granularity = sw.TrimmingGranularity.Character }, null);
					break;
				case FormattedTextTrimming.CharacterEllipsis:
					layout.SetTrimming(new sw.Trimming { Granularity = sw.TrimmingGranularity.Character }, new sw.EllipsisTrimming(SDFactory.DirectWriteFactory, layout));
					break;
				case FormattedTextTrimming.WordEllipsis:
					layout.SetTrimming(new sw.Trimming { Granularity = sw.TrimmingGranularity.Word }, new sw.EllipsisTrimming(SDFactory.DirectWriteFactory, layout));
					break;
				default:
					break;
			}
		}

		public FormattedTextAlignment Alignment
		{
			get => _alignment;
			set
			{
				if (_alignment != value)
				{
					_alignment = value;
					if (HasControl)
						SetAlignment(Control);
				}
			}
		}

		private void SetAlignment(sw.TextLayout layout)
		{
			switch (_alignment)
			{
				case FormattedTextAlignment.Left:
					layout.TextAlignment = sw.TextAlignment.Leading;
					break;
				case FormattedTextAlignment.Right:
					layout.TextAlignment = sw.TextAlignment.Trailing;
					break;
				case FormattedTextAlignment.Center:
					layout.TextAlignment = sw.TextAlignment.Center;
					break;
				case FormattedTextAlignment.Justify:
					layout.TextAlignment = sw.TextAlignment.Justified;
					break;
				default:
					break;
			}
		}

		public Font Font
		{
			get => _font;
			set
			{
				_font = value;
				Invalidate();
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
			get => _maximumSize;
			set
			{
				_maximumSize = value;
				if (HasControl)
				{
					Control.MaxWidth = value.Width;
					Control.MaxHeight = value.Height;
				}
			}
		}
		public Brush ForegroundBrush { get; set; } = Brushes.White;

		public SizeF Measure()
		{
			var metrics = Control.Metrics;
			return new SizeF(metrics.WidthIncludingTrailingWhitespace, metrics.Height);
		}

		void Invalidate()
		{
			Control?.Dispose();
			Control = null;
		}

		void SetWrap(sw.TextLayout layout)
		{
			switch (_wrap)
			{
				case FormattedTextWrapMode.None:
					layout.WordWrapping = sw.WordWrapping.NoWrap;
					break;
				case FormattedTextWrapMode.Word:
					layout.WordWrapping = sw.WordWrapping.WholeWord;
					break;
				case FormattedTextWrapMode.Character:
					layout.WordWrapping = sw.WordWrapping.Character;
					break;
			}
		}

		protected override sw.TextLayout CreateControl()
		{
			var format = new sw.TextFormat(SDFactory.DirectWriteFactory, _font.FamilyName, _font.Size * 96.0f / 72.0f);
			var layout = new sw.TextLayout(SDFactory.DirectWriteFactory, _text, format, _maximumSize.Width, _maximumSize.Height);
			SetWrap(layout);
			SetAlignment(layout);
			SetTrimming(layout);
			return layout;
		}

		internal void Draw(GraphicsHandler graphicsHandler, PointF origin)
		{
			graphicsHandler.Control.DrawTextLayout(origin.ToDx(), Control, ForegroundBrush.ToDx(graphicsHandler.Control));
		}
	}
}
