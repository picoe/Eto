using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using Eto.Wpf.Forms.Controls;
using swd = System.Windows.Documents;
using sw = System.Windows;

namespace Eto.Wpf.Forms
{
	public class TextBufferHandler : WidgetHandler<swd.FlowDocument, TextBuffer>, TextBuffer.IHandler
	{
		static RichTextAreaFormat[] supportedFormats;

		public IEnumerable<RichTextAreaFormat> SupportedFormats => supportedFormats ?? (supportedFormats = new[] { RichTextAreaFormat.PlainText, RichTextAreaFormat.Rtf });

		swd.TextRange ContentRange => new swd.TextRange(Control.ContentStart, Control.ContentEnd);

		public int TextLength => ContentRange.GetLength();

		public TextBufferHandler()
		{
			Control = new swd.FlowDocument();
		}

		swd.TextPointer GetPointer(int offset) => Control.GetTextPositionAtOffset(offset);
		swd.TextRange GetRange(Range<int> range) => Control.GetRange(range);

		void SetRange(Range<int> range, Action<swd.TextRange> action)
		{
			action(GetRange(range));
		}

		public void Append(string text)
		{
			var range = new swd.TextRange(Control.ContentEnd, Control.ContentEnd);
			range.Text = text;
		}

		public void Append(ITextBuffer buffer)
		{
			Control.Blocks.AddRange(buffer.ToWpf().Blocks.ToList());
		}

		public void BeginEdit()
		{
		}

		public void Clear() => Control.Blocks.Clear();

		public void Delete(Range<int> range)
		{
			var textRange = GetRange(range);
			textRange.Text = string.Empty;
		}

		public void EndEdit()
		{
		}

		public void Insert(int position, string text)
		{
			var pos = GetPointer(position);
			pos.InsertTextInRun(text);
		}

		public void Insert(int position, ITextBuffer buffer)
		{
			throw new NotImplementedException();
		}

		public void Load(Stream stream, RichTextAreaFormat format)
		{
			throw new NotImplementedException();
		}

		public void Save(Stream stream, RichTextAreaFormat format)
		{
			throw new NotImplementedException();
		}

		public void SetBackground(Range<int> range, Color color)
		{
			var brush = color.ToWpfBrush();
			SetRange(range, tr => tr.ApplyPropertyValue(swd.TextElement.BackgroundProperty, brush));
		}

		public void SetBold(Range<int> range, bool bold)
		{
			var weight = bold ? sw.FontWeights.Bold : sw.FontWeights.Normal;
			SetRange(range, tr => tr.ApplyPropertyValue(swd.TextElement.FontWeightProperty, weight));
		}

		public void SetFamily(Range<int> range, FontFamily family)
		{
			SetRange(range, tr => tr.SetEtoFamily(family));
		}

		public void SetFont(Range<int> range, Font font)
		{
			SetRange(range, tr => tr.SetEtoFont(font));
		}

		public void SetForeground(Range<int> range, Color color)
		{
			var brush = color.ToWpfBrush();
			SetRange(range, tr => tr.ApplyPropertyValue(swd.TextElement.ForegroundProperty, brush));
		}

		public void SetItalic(Range<int> range, bool italic)
		{
			var style = italic ? sw.FontStyles.Italic : sw.FontStyles.Normal;
			SetRange(range, tr => tr.ApplyPropertyValue(swd.TextElement.FontStyleProperty, style));
		}

		public void SetStrikethrough(Range<int> range, bool strikethrough)
		{
			SetRange(range, tr => tr.SetDecorations(sw.TextDecorations.Strikethrough, strikethrough));
		}

		public void SetUnderline(Range<int> range, bool underline)
		{
			SetRange(range, tr => tr.SetDecorations(sw.TextDecorations.Underline, underline));
		}
	}
}
