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

	// Shrinking a wrapped TextArea must re-measure its template at the new height. The re-layout
	// used the height from the *previous* pass, so while shrinking the inner ScrollViewer and its
	// ScrollContentPresenter kept the old, larger size: WPF arranges children that measured larger
	// than their arrange rect at their unclipped desired size, and TextBox does not clip its
	// template, so the text was drawn outside the control on top of whatever sat below it. The
	// stale ViewportHeight also made ScrollToEnd() stop short of the end, leaving the last line
	// hanging outside the control.
	[Test]
	public void ShrinkingWrappedTextAreaShouldNotLeaveScrollViewerAtOldSize()
	{
		Form form = null;
		Panel spacer = null;
		Shown(f =>
		{
			form = f;
			form.ClientSize = new Size(250, 200);
			var textArea = new TextArea { Wrap = true, Border = BorderType.None, Text = LongWrappingText() };
			spacer = new Panel { Height = 20 };
			form.Content = new TableLayout
			{
				Rows =
				{
					new TableRow(textArea) { ScaleHeight = true },
					spacer
				}
			};
			return textArea;
		}, textArea =>
		{
			var box = ((TextAreaHandler)textArea.Handler).Control;
			box.UpdateLayout();
			var initialHeight = box.ActualHeight;
			Assert.That(box.ViewportHeight, Is.EqualTo(initialHeight).Within(0.01), "#1 viewport should start out matching the control");

			// grow what is below the text area so it gets squeezed into a smaller row
			spacer.Height = 120;
			form.UpdateLayout();
			box.UpdateLayout();

			Assert.That(box.ActualHeight, Is.LessThan(initialHeight), "#2 text area should have shrunk");

			var contentHost = box.Template.FindName("PART_ContentHost", box) as swc.ScrollViewer;
			Assert.That(contentHost, Is.Not.Null, "#3 should have a content host");
			Assert.That(contentHost.ActualHeight, Is.LessThanOrEqualTo(box.ActualHeight + 0.01),
				"#4 the template must not be left taller than the control, it is not clipped and would draw outside it");

			Assert.That(box.ViewportHeight, Is.EqualTo(box.ActualHeight).Within(0.01),
				"#5 the viewport must follow the new size, otherwise scrolling uses the old height");

			box.ScrollToEnd();
			box.UpdateLayout();
			// tolerance covers ScrollToEnd landing on a line boundary a fraction of a pixel short;
			// scrolling against a stale viewport stops short by a whole line or more
			Assert.That(box.VerticalOffset, Is.EqualTo(box.ExtentHeight - box.ActualHeight).Within(1),
				"#6 scrolling to the end should put the end of the text at the bottom of the control");
		});
	}

	// The text must wrap at the width the control is *displayed* at, not the narrower width it was
	// measured at. An auto-sizing window probes its content with the monitor work area, and the
	// handler clamps that to the control's preferred size (100x60) so the text area doesn't grow to
	// its content - a WPF TextBox wraps its text at the width it was measured at, so without the
	// re-layout in EtoTextBox.MeasureOverride the text stays wrapped at ~100px however wide the
	// control actually ends up.
	[Test]
	public void WrappedTextAreaShouldWrapAtDisplayedWidth() => Shown(form =>
	{
		form.AutoSize = true;
		var textArea = new TextArea { Wrap = true, Text = LongWrappingText() };
		// the sibling makes the auto-sized window - and so the text area - much wider than the
		// preferred width the text area is measured at
		form.Content = new TableLayout
		{
			Rows =
			{
				new TableRow(new Panel { Width = 500, Height = 20 }),
				new TableRow(textArea) { ScaleHeight = true }
			}
		};
		return textArea;
	}, textArea =>
	{
		var box = ((TextAreaHandler)textArea.Handler).Control;
		box.UpdateLayout();

		Assert.That(box.TextWrapping, Is.EqualTo(sw.TextWrapping.Wrap), "#1 text should be wrapping");
		Assert.That(box.ViewportWidth, Is.GreaterThan(400),
			"#2 the text area should be stretched well past the preferred width it is measured at");
		// wrapped at the preferred width instead, the extent is ~100px wide in a ~480px viewport
		Assert.That(box.ExtentWidth, Is.GreaterThan(box.ViewportWidth * 0.8),
			"#3 text should be wrapped to the displayed width, not the width it was measured at");
	});

	// The width axis needs no such guard in MeasureOverride: measuring the template at the stale
	// (wider) ActualWidth does leave it too wide for one pass, but ArrangeOverride then sees the
	// arranged width no longer matches the width the text was laid out at and invalidates the
	// measure, so a corrective pass follows. That feedback is what keeps this test passing - there
	// is no equivalent for the height, hence the asymmetry in MeasureOverride.
	[Test]
	public void ShrinkingWrappedTextAreaShouldNotLeaveScrollViewerAtOldWidth()
	{
		Form form = null;
		Panel spacer = null;
		Shown(f =>
		{
			form = f;
			form.ClientSize = new Size(400, 200);
			var textArea = new TextArea { Wrap = true, Border = BorderType.None, Text = LongWrappingText() };
			spacer = new Panel { Width = 20 };
			form.Content = new TableLayout { Rows = { new TableRow(new TableCell(textArea, true), spacer) } };
			return textArea;
		}, textArea =>
		{
			var box = ((TextAreaHandler)textArea.Handler).Control;
			box.UpdateLayout();
			var initialWidth = box.ActualWidth;

			// grow what is beside the text area so it gets squeezed into a narrower column
			spacer.Width = 220;
			form.UpdateLayout();
			box.UpdateLayout();

			Assert.That(box.ActualWidth, Is.LessThan(initialWidth), "#1 text area should have shrunk");

			var contentHost = box.Template.FindName("PART_ContentHost", box) as swc.ScrollViewer;
			Assert.That(contentHost, Is.Not.Null, "#2 should have a content host");
			Assert.That(contentHost.ActualWidth, Is.LessThanOrEqualTo(box.ActualWidth + 0.01),
				"#3 the template must not be left wider than the control, it is not clipped and would draw outside it");

			Assert.That(box.ViewportWidth, Is.LessThanOrEqualTo(box.ActualWidth),
				"#4 the viewport must follow the new size, otherwise it reports the old width");

			Assert.That(box.ExtentWidth, Is.LessThanOrEqualTo(box.ActualWidth),
				"#5 the text must be re-wrapped to the narrower width instead of spilling out to the right");
		});
	}
}
