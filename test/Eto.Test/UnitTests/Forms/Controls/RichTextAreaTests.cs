using System;
using Eto.Forms;
using NUnit.Framework;
using Eto.Drawing;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class RichTextAreaTests : TextAreaTests<RichTextArea>
	{

		[Test]
		public void CheckSelectionTextCaretAfterSettingRtf()
		{
			Invoke(() =>
			{
				// not supported in GTK yet
				if (Platform.Instance.IsGtk)
					Assert.Inconclusive("Gtk does not support RTF format");
				int selectionChanged = 0;
				int textChanged = 0;
				int caretChanged = 0;
				string val;
				var textArea = new RichTextArea();
				textArea.TextChanged += (sender, e) => textChanged++;
				textArea.SelectionChanged += (sender, e) => selectionChanged++;
				textArea.CaretIndexChanged += (sender, e) => caretChanged++;
				Assert.AreEqual(Range.FromLength(0, 0), textArea.Selection, "#1");

				textArea.Rtf = @"{\rtf1\ansi {Hello \ul Underline \i Italic \b Bold \strike Strike}}";
				Assert.AreEqual(val = "Hello Underline Italic Bold Strike", textArea.Text.TrimEnd(), "#2-1");
				Assert.AreEqual(Range.FromLength(val.Length, 0), textArea.Selection, "#2-2");
				Assert.AreEqual(val.Length, textArea.CaretIndex, "#2-3");
				Assert.AreEqual(1, textChanged, "#2-4");
				Assert.AreEqual(1, selectionChanged, "#2-5");
				Assert.AreEqual(1, caretChanged, "#2-6");

				textArea.Selection = Range.FromLength(6, 5);
				Assert.AreEqual(Range.FromLength(6, 5), textArea.Selection, "#3-1");
				Assert.AreEqual(6, textArea.CaretIndex, "#3-2");
				Assert.AreEqual(1, textChanged, "#3-3");
				Assert.AreEqual(2, selectionChanged, "#3-4");
				Assert.AreEqual(2, caretChanged, "#3-5");

				textArea.Rtf = @"{\rtf1\ansi {Some \b other \i text}}";
				Assert.AreEqual(val = "Some other text", textArea.Text.TrimEnd(), "#4-1");
				Assert.AreEqual(Range.FromLength(val.Length, 0), textArea.Selection, "#4-2");
				Assert.AreEqual(val.Length, textArea.CaretIndex, "#4-3");
				Assert.AreEqual(2, textChanged, "#4-4");
				Assert.AreEqual(3, selectionChanged, "#4-5");
				Assert.AreEqual(3, caretChanged, "#4-6");
			});
		}

		public static void TestSelectionAttributes(RichTextArea richText, string tag, bool italic = false, bool underline = false, bool bold = false, bool strikethrough = false)
		{
			Assert.AreEqual(italic, richText.SelectionItalic, tag + "-1");
			Assert.AreEqual(underline, richText.SelectionUnderline, tag + "-2");
			Assert.AreEqual(bold, richText.SelectionBold, tag + "-3");
			Assert.AreEqual(strikethrough, richText.SelectionStrikethrough, tag + "-4");
		}

		[Test]
		public void SelectionAttributesShouldBeCorrectWithLoadedRtf()
		{
			Invoke(() =>
			{
				// not supported in GTK yet
				if (Platform.Instance.IsGtk)
					Assert.Inconclusive("Gtk does not support RTF format");

				var richText = new RichTextArea();
				richText.Rtf = @"{\rtf1\ansi {Hello \ul Underline \i Italic \b Bold \strike Strike}}";
				Assert.AreEqual("Hello Underline Italic Bold Strike", richText.Text.TrimEnd(), "#1");
				richText.CaretIndex = 5;
				TestSelectionAttributes(richText, "#2");
				richText.CaretIndex = 7;
				TestSelectionAttributes(richText, "#3", underline: true);
				richText.CaretIndex = 17;
				TestSelectionAttributes(richText, "#4", underline: true, italic: true);
				richText.CaretIndex = 24;
				TestSelectionAttributes(richText, "#5", underline: true, italic: true, bold: true);
				richText.CaretIndex = 29;
				TestSelectionAttributes(richText, "#6", underline: true, italic: true, bold: true, strikethrough: true);
			});
		}

		[Test]
		public void NewLineAtEndShouldNotBeRemoved()
		{
			Invoke(() =>
			{
				string val;
				var richText = new RichTextArea();
				// winforms always returns \n instead of \r\n.. why??
				var nl = Platform.Instance.IsWinForms ? "\n" : Environment.NewLine;

				if (!Platform.Instance.IsWpf)
				{
					// why does WPF always add a newline even when the content doesn't have a newline?
					richText.Text = val = $"This is{nl}some text";
					Assert.AreEqual(val, richText.Text, "#1");
				}

				richText.Text = val = $"This is{nl}some text{nl}";
				Assert.AreEqual(val, richText.Text, "#2");
			});
		}

		[Test]
		public void SelectionBoldItalicUnderlineShouldTriggerTextChanged()
		{
			Invoke(() =>
			{
				int textChangedCount = 0;
				var richText = new RichTextArea();
				richText.TextChanged += (sender, e) => textChangedCount++;

				string text = "This is some underline, strikethrough, bold, and italic text. This is green, background blue text.";

				Range<int> GetRange(string s) => Range.FromLength(text.IndexOf(s, StringComparison.Ordinal), s.Length);

				richText.Text = text;
				Assert.AreEqual(1, textChangedCount);

				richText.Selection = GetRange("underline");
				richText.SelectionUnderline = true;
				Assert.AreEqual(2, textChangedCount, "RichTextArea.TextChanged did not fire when setting SelectionUnderline");
				Assert.AreEqual(true, richText.SelectionUnderline);
				Assert.AreEqual(false, richText.SelectionStrikethrough);
				Assert.AreEqual(false, richText.SelectionBold);
				Assert.AreEqual(false, richText.SelectionItalic);

				richText.Selection = GetRange("strikethrough");
				richText.SelectionStrikethrough = true;
				Assert.AreEqual(3, textChangedCount, "RichTextArea.TextChanged did not fire when setting SelectionStrikethrough");
				Assert.AreEqual(false, richText.SelectionUnderline);
				Assert.AreEqual(true, richText.SelectionStrikethrough);
				Assert.AreEqual(false, richText.SelectionBold);
				Assert.AreEqual(false, richText.SelectionItalic);

				richText.Selection = GetRange("bold");
				richText.SelectionBold = true;
				Assert.AreEqual(4, textChangedCount, "RichTextArea.TextChanged did not fire when setting SelectionBold");
				Assert.AreEqual(false, richText.SelectionUnderline);
				Assert.AreEqual(false, richText.SelectionStrikethrough);
				Assert.AreEqual(true, richText.SelectionBold);
				Assert.AreEqual(false, richText.SelectionItalic);

				richText.Selection = GetRange("italic");
				richText.SelectionItalic = true;
				Assert.AreEqual(5, textChangedCount, "RichTextArea.TextChanged did not fire when setting SelectionItalic");
				Assert.AreEqual(false, richText.SelectionUnderline);
				Assert.AreEqual(false, richText.SelectionStrikethrough);
				Assert.AreEqual(false, richText.SelectionBold);
				Assert.AreEqual(true, richText.SelectionItalic);

				richText.Selection = GetRange("green");
				richText.SelectionForeground = Colors.Green;
				Assert.AreEqual(6, textChangedCount, "RichTextArea.TextChanged did not fire when setting SelectionForeground");
				Assert.AreEqual(Colors.Green, richText.SelectionForeground);

				richText.Selection = GetRange("green");
				richText.SelectionBackground = Colors.Blue;
				Assert.AreEqual(7, textChangedCount, "RichTextArea.TextChanged did not fire when setting SelectionBackground");
				Assert.AreEqual(Colors.Blue, richText.SelectionBackground);
			});
		}
	}
}
