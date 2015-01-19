using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Documents;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;
using Eto.Wpf.Drawing;
using System.Globalization;
using System.Collections;
using System.IO;

namespace Eto.Wpf.Forms.Controls
{
	public class RichTextAreaHandler : TextAreaHandler<swc.RichTextBox, RichTextArea, RichTextArea.ICallback>, RichTextArea.IHandler, ITextBuffer
	{
		public override string Text
		{
			get
			{
				var doc = Control.Document;
				var textRange = new swd.TextRange(doc.ContentStart, doc.ContentEnd);
				return textRange.Text;
			}
			set
			{
				var doc = Control.Document;
				var textRange = new swd.TextRange(doc.ContentStart, doc.ContentEnd);
				textRange.Text = value ?? string.Empty;
			}
		}

		bool wrap = true;
		public override bool Wrap
		{
			get { return wrap; }
			set
			{
				if (value != wrap)
				{
					wrap = value;
					if (wrap)
					{
						Control.TextChanged -= Control_TextChangedSetPageWidth;
						Control.Document.PageWidth = double.NaN;
					}
					else
					{
						Control.TextChanged += Control_TextChangedSetPageWidth;
						SetPageWidthToContent();
					}
				}
			}
		}

		void Control_TextChangedSetPageWidth(object sender, swc.TextChangedEventArgs e)
		{
			SetPageWidthToContent();
		}

		void SetPageWidthToContent()
		{
			Control.Document.PageWidth = Control.Document.GetFormattedText().WidthIncludingTrailingWhitespace + 20;
		}

		public override string SelectedText
		{
			get { return Control.Selection.Text; }
			set { Control.Selection.Text = value; }
		}

		public override Range<int> Selection
		{
			get
			{
				var sel = Control.Selection;
				return new Range<int>(sel.Start.GetTextOffset(), sel.End.GetTextOffset());
			}
			set
			{
				var contentStart = Control.Document.ContentStart;
				Control.Selection.Select(contentStart.GetTextPositionAtOffset(value.Start), contentStart.GetTextPositionAtOffset(value.End + 1));
			}
		}

		public override int CaretIndex
		{
			get { return Control.CaretPosition.GetTextOffset(); }
			set { Control.CaretPosition = Control.Document.ContentStart.GetTextPositionAtOffset(value); }
		}

		public Font SelectionFont
		{
			get { return new Font(new FontHandler(Control.Selection, Control)); }
			set { Control.Selection.SetEtoFont(value); }
		}

		public Color SelectionForeground
		{
			get
			{
				var brush = Control.Selection.GetPropertyValue(swd.TextElement.ForegroundProperty) as swm.Brush;
				return brush.ToEtoColor();
			}
			set
			{
				Control.Selection.ApplyPropertyValue(swd.TextElement.ForegroundProperty, value.ToWpfBrush());
			}
		}

		public Color SelectionBackground
		{
			get
			{
				var brush = Control.Selection.GetPropertyValue(swd.TextElement.BackgroundProperty) as swm.Brush;
				return brush.ToEtoColor();
			}
			set
			{
				Control.Selection.ApplyPropertyValue(swd.TextElement.BackgroundProperty, value.ToWpfBrush());
			}
		}

		swd.TextRange GetRange(Range<int> range)
		{
			var content = Control.Document.ContentStart;
			return new swd.TextRange(content.GetTextPositionAtOffset(range.Start), content.GetTextPositionAtOffset(range.End + 1));
		}

		void SetRange(Range<int> range, Action<swd.TextRange> action)
		{
			action(GetRange(range));
		}

		public void SetFont(Range<int> range, Font font)
		{
			SetRange(range, tr => tr.SetEtoFont(font));
		}

		public void SetForeground(Range<int> range, Color color)
		{
			SetRange(range, tr => tr.ApplyPropertyValue(swd.TextElement.ForegroundProperty, color.ToWpfBrush()));
		}

		public void SetBackground(Range<int> range, Color color)
		{
			SetRange(range, tr => tr.ApplyPropertyValue(swd.TextElement.BackgroundProperty, color.ToWpfBrush()));
		}

		public bool SelectionBold
		{
			get
			{
				var fontWeight = Control.Selection.GetPropertyValue(swd.TextElement.FontWeightProperty) as sw.FontWeight? ?? sw.FontWeights.Normal;
				return fontWeight == sw.FontWeights.Bold
					|| fontWeight == sw.FontWeights.DemiBold
					|| fontWeight == sw.FontWeights.ExtraBold
					|| fontWeight == sw.FontWeights.SemiBold
					|| fontWeight == sw.FontWeights.UltraBold;
			}
			set
			{
				Control.Selection.ApplyPropertyValue(swd.TextElement.FontWeightProperty, value ? sw.FontWeights.Bold : sw.FontWeights.Normal);
			}
		}

		public void SetBold(Range<int> range, bool bold)
		{
			SetRange(range, tr => tr.ApplyPropertyValue(swd.TextElement.FontWeightProperty, bold ? sw.FontWeights.Bold : sw.FontWeights.Normal));
		}


		public bool SelectionItalic
		{
			get
			{
				var fontStyle = Control.Selection.GetPropertyValue(swd.TextElement.FontStyleProperty) as sw.FontStyle? ?? sw.FontStyles.Normal;
				return fontStyle != sw.FontStyles.Normal;
			}
			set
			{
				Control.Selection.ApplyPropertyValue(swd.TextElement.FontStyleProperty, value ? sw.FontStyles.Italic : sw.FontStyles.Normal);
			}
		}

		// Existing GetPropertyValue for the TextDecorationCollection will return the first collection, even if it is empty.
		// this skips empty collections so we can get the actual value.
		// slightly modified code from https://social.msdn.microsoft.com/Forums/vstudio/en-US/3ac626cf-60aa-427f-80e9-794f3775a70e/how-to-tell-if-richtextbox-selection-is-underlined?forum=wpf
		object GetPropertyValue(swd.TextRange textRange, sw.DependencyProperty formattingProperty)
		{
			object value = null;
			var pointer = textRange.Start as swd.TextPointer;
			if (pointer != null)
			{
				var needsContinue = true;
				sw.DependencyObject element = pointer.Parent as swd.TextElement;
				while (needsContinue && (element is swd.Inline || element is swd.Paragraph || element is swc.TextBlock))
				{
					value = element.GetValue(formattingProperty);
					var seq = value as IEnumerable;
					needsContinue = (seq == null) ? value == null : seq.Cast<Object>().Count() == 0;
					element = element is swd.TextElement ? ((swd.TextElement)element).Parent : null;
				}
			}
			return value;
		}

		bool HasDecorations(swd.TextRange range, sw.TextDecorationCollection decorations)
		{
			var existingDecorations = GetPropertyValue(range, swd.Inline.TextDecorationsProperty) as sw.TextDecorationCollection;
			return existingDecorations != null && decorations.All(r => existingDecorations.Contains(r));
		}

		void SetDecorations(swd.TextRange range, sw.TextDecorationCollection decorations, bool value)
		{
			var existingDecorations = range.GetPropertyValue(swd.Inline.TextDecorationsProperty) as sw.TextDecorationCollection;
			if (existingDecorations != null)
				existingDecorations = new sw.TextDecorationCollection(existingDecorations);
			if (value)
			{
				existingDecorations = existingDecorations ?? new sw.TextDecorationCollection();
				existingDecorations.Add(decorations);
			}
			else if (existingDecorations != null)
			{
				foreach (var decoration in decorations)
				{
					if (existingDecorations.Contains(decoration))
						existingDecorations.Remove(decoration);
				}
				if (existingDecorations.Count == 0)
					existingDecorations = null;
			}
			range.ApplyPropertyValue(swd.Inline.TextDecorationsProperty, existingDecorations);
		}

		public bool SelectionUnderline
		{
			get
			{
				return HasDecorations(Control.Selection, sw.TextDecorations.Underline);
			}
			set
			{
				SetDecorations(Control.Selection, sw.TextDecorations.Underline, value);
			}
		}

		public bool SelectionStrikethrough
		{
			get
			{
				return HasDecorations(Control.Selection, sw.TextDecorations.Strikethrough);
			}
			set
			{
				SetDecorations(Control.Selection, sw.TextDecorations.Strikethrough, value);
			}
		}

		public void SetItalic(Range<int> range, bool italic)
		{
			SetRange(range, tr => tr.ApplyPropertyValue(swd.TextElement.FontStyleProperty, italic ? sw.FontStyles.Italic : sw.FontStyles.Normal));
		}

		public void SetUnderline(Range<int> range, bool underline)
		{
			SetRange(range, tr => SetDecorations(tr, sw.TextDecorations.Underline, underline));
		}

		public void SetStrikethrough(Range<int> range, bool strikethrough)
		{
			SetRange(range, tr => SetDecorations(tr, sw.TextDecorations.Strikethrough, strikethrough));
		}

		public IEnumerable<RichTextAreaFormat> SupportedFormats
		{
			get
			{
				yield return RichTextAreaFormat.Rtf;
				yield return RichTextAreaFormat.PlainText;
			}
		}

		public void Load(Stream stream, RichTextAreaFormat format)
		{
			var range = new swd.TextRange(Control.Document.ContentStart, Control.Document.ContentEnd);
			switch (format)
			{
				case RichTextAreaFormat.Rtf:
					range.Load(stream, sw.DataFormats.Rtf);
					break;
				case RichTextAreaFormat.PlainText:
					range.Load(stream, sw.DataFormats.Text);
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public void Save(Stream stream, RichTextAreaFormat format)
		{
			var range = new swd.TextRange(Control.Document.ContentStart, Control.Document.ContentEnd);
			switch (format)
			{
				case RichTextAreaFormat.Rtf:
					range.Save(stream, sw.DataFormats.Rtf);
					break;
				case RichTextAreaFormat.PlainText:
					range.Save(stream, sw.DataFormats.Rtf);
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public void Clear()
		{
			Text = null;
		}

		public void Delete(Range<int> range)
		{
			var textRange = GetRange(range);
			textRange.Text = null;
		}

		public void Insert(int position, string text)
		{
			var pos = Control.Document.ContentStart.GetTextPositionAtOffset(position);
			pos.InsertTextInRun(text);
		}

		public ITextBuffer Buffer
		{
			get { return this; }
		}
	}

	static class FlowDocumentExtensions
	{
		static IEnumerable<swd.TextElement> GetRunsAndParagraphs(swd.FlowDocument doc)
		{
			for (var position = doc.ContentStart;
			  position != null && position.CompareTo(doc.ContentEnd) <= 0;
			  position = position.GetNextContextPosition(swd.LogicalDirection.Forward))
			{
				if (position.GetPointerContext(swd.LogicalDirection.Forward) == swd.TextPointerContext.ElementEnd)
				{
					var run = position.Parent as swd.Run;

					if (run != null)
						yield return run;
					else
					{
						var para = position.Parent as swd.Paragraph;

						if (para != null)
							yield return para;
					}
				}
			}
		}

		// https://social.msdn.microsoft.com/Forums/sharepoint/en-US/6cd49173-b06d-4749-85aa-f6ab46c7d4af/wpf-rich-text-box-width-size-adjust-to-text?forum=wpf
		public static swm.FormattedText GetFormattedText(this swd.FlowDocument doc)
		{
			if (doc == null)
				throw new ArgumentNullException("doc");

			var output = new swm.FormattedText(
			  GetText(doc),
			  CultureInfo.CurrentCulture,
			  doc.FlowDirection,
			  new swm.Typeface(doc.FontFamily, doc.FontStyle, doc.FontWeight, doc.FontStretch),
			  doc.FontSize,
			  doc.Foreground);

			int offset = 0;

			foreach (var el in GetRunsAndParagraphs(doc))
			{
				var run = el as swd.Run;

				if (run != null)
				{
					int count = run.Text.Length;

					output.SetFontFamily(run.FontFamily, offset, count);
					output.SetFontStyle(run.FontStyle, offset, count);
					output.SetFontWeight(run.FontWeight, offset, count);
					output.SetFontSize(run.FontSize, offset, count);
					output.SetForegroundBrush(run.Foreground, offset, count);
					output.SetFontStretch(run.FontStretch, offset, count);
					output.SetTextDecorations(run.TextDecorations, offset, count);

					offset += count;
				}
				else
				{
					offset += Environment.NewLine.Length;
				}
			}

			return output;
		}

		public static string GetText(this swd.FlowDocument doc)
		{
			var sb = new StringBuilder();

			foreach (var el in GetRunsAndParagraphs(doc))
			{
				var run = el as swd.Run;
				sb.Append(run == null ? Environment.NewLine : run.Text);
			}
			return sb.ToString();
		}

		public static int GetTextOffset(this swd.TextPointer position)
		{
			var offset = 0;
			while (position != null)
			{
				if (position.GetPointerContext(swd.LogicalDirection.Backward) == swd.TextPointerContext.Text)
				{
					offset += position.GetTextRunLength(swd.LogicalDirection.Backward);
				}

				var nextContextPosition = position.GetNextContextPosition(swd.LogicalDirection.Backward);
				if (nextContextPosition == null)
					return offset;

				position = nextContextPosition;
			}

			return offset;
		}

		public static swd.TextPointer GetTextPositionAtOffset(this swd.TextPointer position, int characterCount)
		{
			while (position != null)
			{
				if (position.GetPointerContext(swd.LogicalDirection.Forward) == swd.TextPointerContext.Text)
				{
					int count = position.GetTextRunLength(swd.LogicalDirection.Forward);
					if (count >= characterCount)
					{
						return position.GetPositionAtOffset(characterCount);
					}

					characterCount -= count;
				}

				var nextContextPosition = position.GetNextContextPosition(swd.LogicalDirection.Forward);
				if (nextContextPosition == null)
					return position;

				position = nextContextPosition;
			}

			return position;
		}
	}
}
