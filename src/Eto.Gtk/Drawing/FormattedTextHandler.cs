using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

namespace Eto.GtkSharp.Drawing
{
	public class FormattedTextHandler : WidgetHandler<object, FormattedText, FormattedText.ICallback>, FormattedText.IHandler
	{
		FormattedTextWrapMode _wrap;
		FormattedTextTrimming _trimming;
		string _text;
		SizeF _maximumSize = SizeF.MaxValue;
		Font _font;
		Brush _foregroundBrush = new SolidBrush(SystemColors.ControlText);
		FormattedTextAlignment _alignment;
		int _maximumLineCount;
		bool _shouldClip;

		public FormattedTextWrapMode Wrap
		{
			get => _wrap;
			set
			{
				if (_wrap != value)
				{
					_wrap = value;
					Invalidate();
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
					Invalidate();
				}
			}
		}
		public string Text
		{
			get => _text;
			set
			{
				if (_text != value)
				{
					_text = value;
					Invalidate();
				}
			}
		}

		public SizeF MaximumSize
		{
			get => _maximumSize;
			set
			{
				if (_maximumSize != value)
				{
					_maximumSize = value;
					Invalidate();
				}
			}
		}
		
		public Font Font
		{
			get => _font;
			set
			{
				if (_font != value)
				{
					_font = value;
					Invalidate();
				}
			}
		}
		
		public Brush ForegroundBrush
		{
			get => _foregroundBrush;
			set
			{
				if (_foregroundBrush != value)
				{
					_foregroundBrush = value;
					Invalidate();
				}
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
					Invalidate();
				}
			}
		}
		public int MaximumLineCount
		{
			get => _maximumLineCount;
			set
			{
				if (_maximumLineCount != value)
				{
					_maximumLineCount = value;
					Invalidate();
				}
			}
		}
		
		void Invalidate()
		{
			_layout?.Dispose();
			_layout = null;
		}

		public SizeF Measure()
		{
			// can we do this more lightweight than creating a control?
			if (_layout == null)
			{
				EnsureLayout(new Gtk.Label().PangoContext);
			}
			_layout.GetPixelSize(out var width, out var height);
			var size = new SizeF(width, height);
			if (Wrap == FormattedTextWrapMode.None && IsUnlimited(MaximumSize.Width))
				size.Width = Math.Min(MaximumSize.Width, width);
			return size;
		}

		const int MaxLayoutSize = 1000000;

		void Setup(Pango.Layout layout)
		{
			Font.Apply(layout);
			var hasNewlines = Text.IndexOf('\n') != -1;
			_shouldClip = false;
			var isRightOrCenter = Alignment == FormattedTextAlignment.Right || Alignment == FormattedTextAlignment.Center;
			var size = SizeF.Min(MaximumSize, new SizeF(MaxLayoutSize, MaxLayoutSize));
			if (size.Width <= 0)
				size.Width = MaxLayoutSize;
			if (size.Height <= 0)
				size.Height = MaxLayoutSize;

			layout.Width = (int)(size.Width * Pango.Scale.PangoScale);
				
#if GTK3
			layout.Height = (int)(size.Height * Pango.Scale.PangoScale);
#endif
			layout.Ellipsize = Trimming == FormattedTextTrimming.None ? Pango.EllipsizeMode.None : Pango.EllipsizeMode.End;
			switch (Wrap)
			{
				case FormattedTextWrapMode.None:
					layout.Wrap = Pango.WrapMode.Char;

					if (Trimming == FormattedTextTrimming.None || hasNewlines || (isRightOrCenter && !hasNewlines))
					{
						_shouldClip = true;
						layout.Width = (int)(MaxLayoutSize * Pango.Scale.PangoScale);
					}
					// if (_shouldClip)
					// 	layout.Width = (int)(MaxLayoutSize * Pango.Scale.PangoScale);
#if GTK3
					// only draw a single line so we can do ellipsizing
					if (!hasNewlines)
						layout.Height = layout.FontDescription.Size;
#endif

					break;
				case FormattedTextWrapMode.Word:
					layout.Wrap = Pango.WrapMode.Word;
					break;
				case FormattedTextWrapMode.Character:
					layout.Wrap = Pango.WrapMode.Char;
					break;
			}
			switch (Alignment)
			{
				case FormattedTextAlignment.Left:
					layout.Alignment = Pango.Alignment.Left;
					break;
				case FormattedTextAlignment.Right:
					layout.Alignment = Pango.Alignment.Right;
					break;
				case FormattedTextAlignment.Center:
					layout.Alignment = Pango.Alignment.Center;
					break;
				case FormattedTextAlignment.Justify:
					layout.Alignment = Pango.Alignment.Left;
					layout.Justify = true;
					break;
			}
			layout.SetText(Text);
			if ((layout.Width >= MaxLayoutSize || !hasNewlines) && isRightOrCenter)
			{
				layout.GetPixelSize(out var width, out var height);
				if (hasNewlines)
					layout.Width = (int)(width * Pango.Scale.PangoScale);
				else if (IsUnlimited(MaximumSize.Width))
					layout.Width = (int)(Math.Max(width, MaximumSize.Width) * Pango.Scale.PangoScale);
				else
					layout.Width = (int)(Math.Min(width, MaximumSize.Width) * Pango.Scale.PangoScale);
				
			}
			if (Trimming == FormattedTextTrimming.None && layout.LineCount > 1 && IsUnlimited(MaximumSize.Height))
			{
				layout.GetPixelSize(out _, out var height);
				while (layout.LineCount > 1 && height > MaximumSize.Height)
				{
					var lineCount = layout.LineCount;
					var line = layout.GetLine(lineCount - 1);
					var len = line.StartIndex;
					layout.SetText(Text.Substring(0, len));
					if (layout.LineCount == lineCount)
					{
						// need to trim off some characters or words from the current line
						while (layout.LineCount == lineCount && --len > 0)
						{
							if (Wrap == FormattedTextWrapMode.Word)
							{
								// keep going till whitespace for wrapping.
								while (len > 0 && !char.IsWhiteSpace(Text[len-1]))
								{
									len--;
								}
							}
							layout.SetText(Text.Substring(0, len));
						}
					}
					layout.GetPixelSize(out _, out height);
				}
			}
		}

		Pango.Layout _layout;

		bool IsUnlimited(float value) => value < float.MaxValue || value <= 0;

		public void Draw(GraphicsHandler graphics, Cairo.Context context, PointF location)
		{
			EnsureLayout(graphics.PangoContext);
			context.Save();
			if (_shouldClip && IsUnlimited(MaximumSize.Width))
			{
				context.Rectangle(new Cairo.Rectangle(location.X, location.Y, Math.Min(MaxLayoutSize, MaximumSize.Width), Math.Min(MaxLayoutSize, MaximumSize.Height)));
				context.Clip();
			}
			ForegroundBrush.Apply(graphics);
			context.MoveTo(location.X, location.Y);
			Pango.CairoHelper.LayoutPath(context, _layout);
			context.Fill();
			context.Restore();
		}

		private void EnsureLayout(Pango.Context context)
		{
			if (_layout == null || _layout.Context.Handle != context.Handle)
			{
				_layout?.Dispose();
				_layout = new Pango.Layout(context);
				Setup(_layout);
			}
		}
	}
}
