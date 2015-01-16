using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Drawing;
using sd = System.Drawing;
using swf = System.Windows.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class RichTextAreaHandler : TextAreaHandler<RichTextArea, RichTextArea.ICallback>, RichTextArea.IHandler, ITextBuffer
	{
		void SetAttribute(Range<int> range, Action action)
		{
			var oldSelection = Selection;
			SuppressSelectionChanged++;
			try
			{
				Selection = range;
				action();
			}
			finally
			{
				Selection = oldSelection;
				SuppressSelectionChanged--;
			}
		}

		bool FontEquals(sd.Font font1, sd.Font font2)
		{
			return font1.OriginalFontName == font2.OriginalFontName
				&& font1.Style == font2.Style
				&& font1.Size == font2.Size;
		}

		void SetSelectionFontStyle(Func<sd.Font, sd.Font> setFont)
		{
			var sel = Selection;
			if (sel.Length() > 0)
				SetFontStyle(sel, setFont);
			else
				Control.SelectionFont = setFont(Control.SelectionFont);
		}

		void SetFontStyle(Range<int> range, Func<sd.Font, sd.Font> setFont)
		{
			SuppressSelectionChanged++;
			var oldSelection = Selection;
			try
			{
				sd.Font currentFont = null;
				int lastPosition = range.Start;
				for (int i = range.Start; i <= range.End; i++)
				{
					Control.Select(i, 1);
					var font = Control.SelectionFont;
					if (currentFont != null && !FontEquals(currentFont, font))
					{
						Control.Select(lastPosition, i - lastPosition);
						Control.SelectionFont = setFont(currentFont);
						lastPosition = i;
					}
					else if (i == range.End)
					{
						// at end, set font on last range
						Control.Select(lastPosition, i - lastPosition + 1);
						Control.SelectionFont = setFont(font);
					}
					currentFont = font;
				}
			}
			finally
			{
				Selection = oldSelection;
				SuppressSelectionChanged--;
			}
		}

		public void SetFont(Range<int> range, Font font)
		{
			SetAttribute(range, () => Control.SelectionFont = font.ToSD());
		}

		public void SetForeground(Range<int> range, Color color)
		{
			SetAttribute(range, () => Control.SelectionColor = color.ToSD());
		}

		public void SetBackground(Range<int> range, Color color)
		{
			SetAttribute(range, () => Control.SelectionBackColor = color.ToSD());
		}

		public void SetBold(Range<int> range, bool bold)
		{
			SetAttribute(range, () => SelectionBold = bold);
		}
		public void SetItalic(Range<int> range, bool bold)
		{
			SetAttribute(range, () => SelectionBold = bold);
		}
		public void SetUnderline(Range<int> range, bool bold)
		{
			SetAttribute(range, () => SelectionUnderline = bold);
		}

		public void SetStrikethrough(Range<int> range, bool strikethrough)
		{
			SetAttribute(range, () => SelectionStrikethrough = strikethrough);
		}

		public Font SelectionFont
		{
			get { return Control.SelectionFont.ToEto(); }
			set { Control.SelectionFont = value.ToSD(); }
		}

		public Color SelectionForeground
		{
			get { return Control.SelectionColor.ToEto(); }
			set { Control.SelectionColor = value.ToSD(); }
		}

		public Color SelectionBackground
		{
			get { return Control.SelectionBackColor.ToEto(); }
			set { Control.SelectionBackColor = value.ToSD(); }
		}

		public bool SelectionBold
		{
			get { return Control.SelectionFont.Bold; }
			set
			{
				SetSelectionFontStyle(font =>
				{
					if (font.Bold == value)
						return font;
					var style = font.Style;
					if (value)
						style |= sd.FontStyle.Bold;
					else
						style &= ~sd.FontStyle.Bold;
					return new sd.Font(font, style);
				});
			}
		}

		public bool SelectionItalic
		{
			get { return Control.SelectionFont.Italic; }
			set
			{
				SetSelectionFontStyle(font =>
				{
					if (font.Italic == value)
						return font;
					var style = font.Style;
					if (value)
						style |= sd.FontStyle.Italic;
					else
						style &= ~sd.FontStyle.Italic;
					return new sd.Font(font, style);
				});
			}
		}

		public bool SelectionUnderline
		{
			get { return Control.SelectionFont.Underline; }
			set
			{
				SetSelectionFontStyle(font =>
				{
					if (font.Underline == value)
						return font;
					var style = font.Style;
					if (value)
						style |= sd.FontStyle.Underline;
					else
						style &= ~sd.FontStyle.Underline;
					return new sd.Font(font, style);
				});
			}
		}

		public bool SelectionStrikethrough
		{
			get { return Control.SelectionFont.Strikeout; }
			set
			{
				SetSelectionFontStyle(font =>
				{
					if (font.Strikeout == value)
						return font;
					var style = font.Style;
					if (value)
						style |= sd.FontStyle.Strikeout;
					else
						style &= ~sd.FontStyle.Strikeout;
					return new sd.Font(font, style);
				});
			}
		}


		public IEnumerable<RichTextAreaFormat> SupportedFormats
		{
			get
			{
				yield return RichTextAreaFormat.Rtf;
				yield return RichTextAreaFormat.PlainText;
			}
		}

		public void Load(System.IO.Stream stream, RichTextAreaFormat format)
		{
			switch (format)
			{
				case RichTextAreaFormat.Rtf:
					Control.LoadFile(stream, swf.RichTextBoxStreamType.RichText);
					break;
				case RichTextAreaFormat.PlainText:
					Control.LoadFile(stream, swf.RichTextBoxStreamType.PlainText);
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public void Save(System.IO.Stream stream, RichTextAreaFormat format)
		{
			switch (format)
			{
				case RichTextAreaFormat.Rtf:
					Control.SaveFile(stream, swf.RichTextBoxStreamType.RichText);
					break;
				case RichTextAreaFormat.PlainText:
					Control.SaveFile(stream, swf.RichTextBoxStreamType.PlainText);
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public void Clear()
		{
			Control.Clear();
		}

		public void Delete(Range<int> range)
		{
			var oldSelection = Selection;
			Selection = range;
			SelectedText = null;
			Selection = oldSelection;
		}

		public void Insert(int position, string text)
		{
			var oldSelection = Selection;
			Control.Select(position, 0);
			SelectedText = text;
			Selection = oldSelection;
		}

		public ITextBuffer Buffer
		{
			get { return this; }
		}
	}
}
