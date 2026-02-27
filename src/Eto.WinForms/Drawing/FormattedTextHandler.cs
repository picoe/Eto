namespace Eto.WinForms.Drawing
{
	public class FormattedTextHandler : WidgetHandler<sd.StringFormat, FormattedText, FormattedText.ICallback>, FormattedText.IHandler
	{
		FormattedTextAlignment _alignment;
		FormattedTextWrapMode _wrap;
		protected override sd.StringFormat CreateControl()
		{
			var control = new sd.StringFormat();
			control.FormatFlags |= sd.StringFormatFlags.LineLimit;
			return control;
		}

		public FormattedTextWrapMode Wrap
		{
			get => _wrap;
			set
			{
				_wrap = value;
				switch (value)
				{
					case FormattedTextWrapMode.None:
						Control.FormatFlags |= sd.StringFormatFlags.NoWrap;
						break;
					case FormattedTextWrapMode.Word:
						Control.FormatFlags &= ~sd.StringFormatFlags.NoWrap;
						break;
					case FormattedTextWrapMode.Character:
						Control.FormatFlags &= ~sd.StringFormatFlags.NoWrap;
						break;
				}
			}
		}

		public FormattedTextTrimming Trimming
		{
			get
			{
				switch (Control.Trimming)
				{
					case sd.StringTrimming.EllipsisCharacter:
						return FormattedTextTrimming.CharacterEllipsis;
					case sd.StringTrimming.EllipsisWord:
						return FormattedTextTrimming.WordEllipsis;
					default:
						return FormattedTextTrimming.None;
				}
			}
			set
			{
				switch (value)
				{
					case FormattedTextTrimming.None:
						Control.Trimming = sd.StringTrimming.None;
						break;
					case FormattedTextTrimming.CharacterEllipsis:
						Control.Trimming = sd.StringTrimming.EllipsisCharacter;
						break;
					case FormattedTextTrimming.WordEllipsis:
						Control.Trimming = sd.StringTrimming.EllipsisWord;
						break;
					default:
						break;
				}
			}
		}

		Font _font = SystemFonts.Default();
		sd.Font _scaledFont;
		
		public string Text { get; set; }

		public SizeF MaximumSize { get; set; } = SizeF.MaxValue;
		public Font Font
		{
			get => _font;
			set
			{
				_font = value;
				_scaledFont = null;
			}
		}
		public Brush ForegroundBrush { get; set; } = new SolidBrush(SystemColors.ControlText);

		public FormattedTextAlignment Alignment
		{
			get => _alignment;
			set
			{
				_alignment = value;
				switch (value)
				{
					case FormattedTextAlignment.Justify:
					case FormattedTextAlignment.Left:
						Control.Alignment = sd.StringAlignment.Near;
						break;
					case FormattedTextAlignment.Right:
						Control.Alignment = sd.StringAlignment.Far;
						break;
					case FormattedTextAlignment.Center:
						Control.Alignment = sd.StringAlignment.Center;
						break;
				}
			}
		}


		public int MaximumLineCount { get; set; }

		sd.Graphics measureGraphics;


		public SizeF Measure()
		{
			if (measureGraphics == null)
			{
				var bmp = new sd.Bitmap(1, 1);
				bmp.SetResolution(96, 96);
				measureGraphics = sd.Graphics.FromImage(bmp);
			}
			var size = measureGraphics.MeasureString(Text, Font.ToSD(), MaximumSize.ToSD(), Control);
			return size.ToEto();
		}

		void DrawJustifiedLines(GraphicsHandler graphics, RectangleF rect, sd.Font font)
		{
			var brush = ForegroundBrush.ToSD(rect);
			var text = Text;
			var lineHeight = font.GetHeight(graphics.Control);
			var format = new sd.StringFormat(Control);
			format.Trimming = sd.StringTrimming.None; // no ellipsis
			float y = rect.Top;

			void DrawParagraph(string paragraph)
			{
				string[] words = paragraph.Split(' ');

				// get widths of each word
				float[] wordWidths = new float[words.Length];
				for (int i = 0; i < words.Length; i++)
				{
					var size = graphics.Control.MeasureString(words[i], font);
					wordWidths[i] = size.Width;
				}

				int currentWord = 0;
				while (true)
				{
					float x = rect.Left;
					float lineWidth = 0;
					int wordsToDraw = 0;
					bool justify = false;
					for (int i = currentWord; i < words.Length; i++)
					{
						var width = wordWidths[i];
						if (x + lineWidth + width > MaximumSize.Width)
						{
							justify = true;
							break;
						}
						wordsToDraw++;
						lineWidth += width;
					}
					if (wordsToDraw == 0)
						break;
					if (!justify)
					{
						var lastLine = string.Join(" ", words.Skip(currentWord).Take(wordsToDraw));
						graphics.Control.DrawString(lastLine, font, brush, x, y, format);
						y += lineHeight;
						break;
					}

					float spacing = rect.Width - lineWidth;
					if (wordsToDraw > 1) spacing /= (wordsToDraw - 1);

					for (int i = 0; i < wordsToDraw; i++)
					{
						var idx = currentWord + i;
						graphics.Control.DrawString(words[idx], font, brush, x, y, format);

						x += wordWidths[idx] + spacing;
					}
					y += lineHeight;

					if (y + lineHeight > rect.Bottom)
						break;

					currentWord += wordsToDraw;
				}
			}


			var lines = text.Split('\n');

			for (int i = 0; i < lines.Length; i++)
			{
				if (y + lineHeight > rect.Bottom)
					break;
					
				string line = lines[i];
				DrawParagraph(line);
			}
		}
		
		public void Draw(GraphicsHandler graphics, PointF location)
		{
			var size = Measure();
			
			_scaledFont ??= graphics.GetScaledFont(Font);

			if (Alignment != FormattedTextAlignment.Left && MaximumSize.Width < int.MaxValue)
				size.Width = MaximumSize.Width;

			var rect = new RectangleF(location.X, location.Y, size.Width, size.Height);
			if (Wrap != FormattedTextWrapMode.None && Alignment == FormattedTextAlignment.Justify && rect.Width < int.MaxValue)
			{
				DrawJustifiedLines(graphics, rect, _scaledFont);
			}
			else
			{
				graphics.Control.DrawString(Text, _scaledFont, ForegroundBrush.ToSD(rect), rect.ToSD(), Control);
			}
		}
	}
}

