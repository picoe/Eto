using Eto.Drawing;
using Eto.Forms;
using Eto.Test.UnitTests;
using Eto.Wpf.Forms.Controls;
using NUnit.Framework;

namespace Eto.Test.Wpf.UnitTests;

[TestFixture]
public class TextAreaWrapScrollTests : TestBase
{
	static string LongWrappingText()
	{
		// long lines that wrap, and lots of them, so the content far exceeds a small viewport
		var sb = new System.Text.StringBuilder();
		for (int i = 0; i < 60; i++)
			sb.AppendLine($"Line {i}: this is a fairly long line of text that should wrap across multiple display lines when the control is narrow enough to force word wrapping.");
		return sb.ToString();
	}

	// A wrapped TextArea whose content is taller than the control must still scroll vertically.
	// Regression test for the wrapping fix, which re-measured the text at an infinite height and
	// caused the internal ScrollViewer to treat the whole content as its viewport (no scrolling).
	[Test]
	public void WrappedTextAreaShouldScrollVertically() => Shown(form =>
	{
		form.ClientSize = new Size(250, 150);
		return new TextArea { Wrap = true, Text = LongWrappingText() };
	}, textArea =>
	{
		var box = ((TextAreaHandler)textArea.Handler).Control;
		box.UpdateLayout();

		Assert.That(box.TextWrapping, Is.EqualTo(sw.TextWrapping.Wrap), "#1 text should be wrapping");
		Assert.That(box.ExtentHeight, Is.GreaterThan(box.ViewportHeight),
			"#2 wrapped content should be taller than the viewport so it can scroll");

		box.ScrollToEnd();
		box.UpdateLayout();
		Assert.That(box.VerticalOffset, Is.GreaterThan(0),
			"#3 scrolling to the end should move the vertical offset");
	});
}
