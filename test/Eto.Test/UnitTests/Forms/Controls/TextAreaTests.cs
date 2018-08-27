using System;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class TextAreaTests : TextAreaTests<TextArea>
	{
	}

	public class TextAreaTests<T> : TestBase
		where T: TextArea, new()
	{
		[Test]
		public void CheckSelectionTextCaretAfterSettingText()
		{
			Invoke(() =>
			{
				int selectionChanged = 0;
				int textChanged = 0;
				string val;
				var textArea = new T();
				textArea.TextChanged += (sender, e) => textChanged++;
				textArea.SelectionChanged += (sender, e) => selectionChanged++;
				Assert.AreEqual(Range.FromLength(0, 0), textArea.Selection, "#1");

				textArea.Text = val = "Hello there";
				Assert.AreEqual(Range.FromLength(val.Length, 0), textArea.Selection, "#2");
				Assert.AreEqual(val.Length, textArea.CaretIndex, "#3");
				Assert.AreEqual(1, textChanged, "#4");
				Assert.AreEqual(1, selectionChanged, "#5");

				textArea.Selection = Range.FromLength(6, 5);
				Assert.AreEqual(Range.FromLength(6, 5), textArea.Selection, "#6");
				Assert.AreEqual(6, textArea.CaretIndex, "#7");
				Assert.AreEqual(1, textChanged, "#8");
				Assert.AreEqual(2, selectionChanged, "#9");

				textArea.Text = val = "Some other text";
				Assert.AreEqual(Range.FromLength(val.Length, 0), textArea.Selection, "#10");
				Assert.AreEqual(val.Length, textArea.CaretIndex, "#11");
				Assert.AreEqual(2, textChanged, "#12");
				Assert.AreEqual(3, selectionChanged, "#13");
			});
		}

		[Test]
		public void EnabledShouldNotAffectReadOnly()
		{
			Invoke(() =>
			{
				var textArea = new T();
				Assert.IsTrue(textArea.Enabled, "#1");
				Assert.IsFalse(textArea.ReadOnly, "#2");
				textArea.Enabled = false;
				Assert.IsFalse(textArea.Enabled, "#3");
				Assert.IsFalse(textArea.ReadOnly, "#4");
				textArea.Enabled = true;
				Assert.IsTrue(textArea.Enabled, "#5");
				Assert.IsFalse(textArea.ReadOnly, "#6");
			});
		}

		[Test]
		public void SettingSelectedTextShouldTriggerTextChanged()
		{
			int textChangedCount = 0;
			int selectionChangedCount = 0;
			Shown(form =>
			{
				var textArea = new T();
				textArea.TextChanged += (sender, e) => textChangedCount++;
				textArea.SelectionChanged += (sender, e) => selectionChangedCount++;
				textArea.Text = "Hello there friend";
				Assert.AreEqual(1, textChangedCount, "#1-1");
				Assert.AreEqual(Range.FromLength(textArea.Text.TrimEnd().Length, 0), textArea.Selection, "#1-2");
				Assert.AreEqual(1, selectionChangedCount, "#1-3");

				textArea.Selection = Range.FromLength(6, 5);
				Assert.AreEqual(1, textChangedCount, "#2-1");
				Assert.AreEqual(2, selectionChangedCount, "#2-2");
				Assert.AreEqual(Range.FromLength(6, 5), textArea.Selection, "#2-3");

				return textArea;

			}, textArea =>
			{
				Assert.AreEqual(1, textChangedCount, "#4-1");
				Assert.AreEqual(2, selectionChangedCount, "#4-2");

				textArea.SelectedText = "my";
				Assert.AreEqual(2, textChangedCount, "#5-1");
				Assert.AreEqual(3, selectionChangedCount, "#5-2");
				Assert.AreEqual("Hello my friend", textArea.Text.TrimEnd(), "#5-3");
				Assert.AreEqual(Range.FromLength(6, 2), textArea.Selection, "#5-4");

				textArea.Selection = textArea.Selection.WithLength(textArea.Selection.Length() + 1);
				Assert.AreEqual(4, selectionChangedCount, "#6");

				textArea.SelectedText = null;
				Assert.AreEqual(3, textChangedCount, "#7-1");
				Assert.AreEqual(5, selectionChangedCount, "#7-2");
				Assert.AreEqual("Hello friend", textArea.Text.TrimEnd(), "#7-3");
				Assert.AreEqual(Range.FromLength(6, 0), textArea.Selection, "#7-4");
			});
		}

		[Test]
		public void InitialValueOfSelectedTextShouldBeEmptyInsteadOfNull()
		{
			Invoke(() =>
			{
				var textArea = new T();
				Assert.AreEqual(string.Empty, textArea.SelectedText, "SelectedText should be empty not null before setting any text");
				textArea.Text = "Hello!";
				Assert.AreEqual(string.Empty, textArea.SelectedText, "SelectedText should *still* be empty not null after setting text");
			});
		}
	}
}
