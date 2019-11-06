using System;
using Eto.Drawing;

namespace Eto.GtkSharp.Drawing
{
	public class FormattedTextHandler : WidgetHandler<object, FormattedText, FormattedText.ICallback>, FormattedText.IHandler
	{
		public FormattedTextWrapMode Wrap { get; set; }
		public FormattedTextTrimming Trimming { get; set; }
		public string Text { get; set; }
		public SizeF MaximumSize { get; set; } = SizeF.MaxValue;
		public Font Font { get; set; }
		public Brush ForegroundBrush { get; set; } = new SolidBrush(SystemColors.ControlText);
		public FormattedTextAlignment Alignment { get; set; }
		public int MaximumLineCount { get; set; }

		public SizeF Measure()
		{
			// can we do this more lightweight than creating a control?
			using (var ctl = new Gtk.Label())
			using (var layout = new Pango.Layout(ctl.PangoContext))
			{
				Setup(layout);
				layout.GetPixelSize(out var width, out var height);
				return new SizeF(width, height);
			}
		}

		void Setup(Pango.Layout layout)
		{
			Font.Apply(layout);
			layout.Width = (int)(MaximumSize.Width * Pango.Scale.PangoScale);
#if GTK3
			layout.Height = (int)(MaximumSize.Height * Pango.Scale.PangoScale);
#endif
			layout.Ellipsize = Trimming == FormattedTextTrimming.None ? Pango.EllipsizeMode.None : Pango.EllipsizeMode.End;
			switch (Wrap)
			{
				case FormattedTextWrapMode.None:
					// only draw one line!!
					layout.Wrap = Pango.WrapMode.Char;
#if GTK3
					layout.Height = (int)((double)layout.FontDescription.Size / (double)Pango.Scale.PangoScale);
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
			if (Wrap == FormattedTextWrapMode.None && layout.LineCount > 1)
			{
				// line includes the full last word so keep shrinking until it isn't wrapped
				var len = layout.GetLine(0).Length;
				while (len > 0 && layout.IsWrapped)
				{
					layout.SetText(Text.Substring(0, len--));
				}
			}
			if (Trimming == FormattedTextTrimming.None && layout.LineCount > 1)
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

		public void Draw(GraphicsHandler graphics, Pango.Layout layout, Cairo.Context context, PointF location)
		{
			Setup(layout);
			context.Save();
			ForegroundBrush.Apply(graphics);
			context.MoveTo(location.X, location.Y);
			Pango.CairoHelper.LayoutPath(context, layout);
			context.Fill();
			context.Restore();
		}
	}
}
