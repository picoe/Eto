using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Eto.Drawing;

namespace Eto.Forms
{
	[Handler(typeof(IHandler))]
	public class TextBuffer : Widget, ITextBuffer
	{
		new IHandler Handler => (IHandler)base.Handler;

		public IEnumerable<RichTextAreaFormat> SupportedFormats => Handler.SupportedFormats;

		public int TextLength => Handler.TextLength;

		public void Append(string text) => Handler.Append(text);

		public void BeginEdit() => Handler.BeginEdit();

		public void Clear() => Handler.Clear();

		public void Delete(Range<int> range) => Handler.Delete(range);

		public void EndEdit() => Handler.EndEdit();

		public void Insert(int position, string text) => Handler.Insert(position, text);

		public void Load(Stream stream, RichTextAreaFormat format) => Handler.Load(stream, format);

		public void Save(Stream stream, RichTextAreaFormat format) => Handler.Save(stream, format);

		public void SetBackground(Range<int> range, Color color) => Handler.SetBackground(range, color);

		public void SetBold(Range<int> range, bool bold) => Handler.SetBold(range, bold);

		public void SetFamily(Range<int> range, FontFamily family) => Handler.SetFamily(range, family);

		public void SetFont(Range<int> range, Font font) => Handler.SetFont(range, font);

		public void SetForeground(Range<int> range, Color color) => Handler.SetForeground(range, color);

		public void SetItalic(Range<int> range, bool italic) => Handler.SetItalic(range, italic);

		public void SetStrikethrough(Range<int> range, bool strikethrough) => Handler.SetStrikethrough(range, strikethrough);

		public void SetUnderline(Range<int> range, bool underline) => Handler.SetUnderline(range, underline);

		public void Insert(int position, ITextBuffer buffer) => Handler.Insert(position, buffer);

		public void Append(ITextBuffer buffer) => Handler.Append(buffer);

		public interface IHandler : ITextBuffer
		{
		}
	}
}
