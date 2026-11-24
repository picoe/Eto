using Eto.Forms;
using Eto.Test.UnitTests;
using Eto.Wpf.Forms.Controls;
using NUnit.Framework;
using swd = System.Windows.Documents;
using Range = Eto.Forms.Range;

namespace Eto.Test.Wpf.UnitTests;

[TestFixture]
public class RichTextAreaSelectionTests : TestBase
{
	// Shift+Enter inserts a LineBreak, which Text reports as a newline, so offsets must count it too.
	[Test]
	[InvokeOnUI]
	public void SelectionShouldCountLineBreaks()
	{
		var richText = new RichTextArea();
		var box = ((RichTextAreaHandler)richText.Handler).Control;
		var paragraph = new swd.Paragraph();
		paragraph.Inlines.Add(new swd.Run("abc"));
		paragraph.Inlines.Add(new swd.LineBreak());
		paragraph.Inlines.Add(new swd.Run("def"));
		box.Document.Blocks.Clear();
		box.Document.Blocks.Add(paragraph);

		Assert.That(richText.Text, Is.EqualTo("abc\ndef\n"), "#1");
		Assert.That(richText.TextLength, Is.EqualTo(7), "#2");

		richText.Selection = Range.FromLength(4, 3);
		Assert.That(richText.SelectedText, Is.EqualTo("def"), "#3.1");
		Assert.That(richText.Selection, Is.EqualTo(Range.FromLength(4, 3)), "#3.2");

		richText.SelectAll();
		Assert.That(richText.Selection, Is.EqualTo(Range.FromLength(0, 7)), "#4");
	}

	// Every selection change used to step through the selection one character at a time (#1616).
	[Test]
	[InvokeOnUI]
	public void ChangingLongSelectionShouldBeFast()
	{
		var richText = new RichTextArea();
		var box = ((RichTextAreaHandler)richText.Handler).Control;
		richText.Text = new string('a', 10000);
		var start = box.Document.ContentStart.GetInsertionPosition(swd.LogicalDirection.Forward);

		var sw = Stopwatch.StartNew();
		for (int i = 0; i < 100; i++)
			box.Selection.Select(start, start.GetPositionAtOffset(9000 + i));
		sw.Stop();

		Assert.That(richText.Selection, Is.EqualTo(Range.FromLength(0, 9099)), "#1");
		Assert.That(sw.ElapsedMilliseconds, Is.LessThan(1000), "#2 selection changes are too slow");
	}
}
