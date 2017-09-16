using System;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class RichTextAreaTests : TestBase
	{

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
		public void EnabledShouldNotAffectReadOnly()
		{
			Invoke(() =>
			{
				var richText = new RichTextArea();
				Assert.IsTrue(richText.Enabled, "#1");
				Assert.IsFalse(richText.ReadOnly, "#2");
				richText.Enabled = false;
				Assert.IsFalse(richText.Enabled, "#3");
				Assert.IsFalse(richText.ReadOnly, "#4");
				richText.Enabled = true;
				Assert.IsTrue(richText.Enabled, "#5");
				Assert.IsFalse(richText.ReadOnly, "#6");
			});
		}
	}
}
