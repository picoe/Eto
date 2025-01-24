using NUnit.Framework;
using Range = Eto.Forms.Range;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class TextAreaTests : TextAreaTests<TextArea>
	{
	}

	public class TextAreaTests<T> : TestBase
		where T : TextArea, new()
	{
		[Test]
		[ManualTest]
		public void ScrollToEndShouldHaveValidStateAfterTextIsInserted() => ManualForm(
			"TextArea should scroll to the end after each time text is inserted,\nand should be fully visible",
			(form, label) =>
		{
			var textArea = new T { Size = new Size(400, 300) };

			form.Shown += async (sender, e) =>
			{
				for (int i = 0; i < 20; i++)
				{
					textArea.Text += LoremGenerator.GenerateLines(10, 100);
					textArea.ScrollToEnd();
					await Task.Delay(200);
				}
			};

			return textArea;
		});
		
		
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
				Assert.That(textArea.Selection, Is.EqualTo(Range.FromLength(0, 0)), "#1");

				textArea.Text = val = "Hello there";
				Assert.That(textArea.Selection, Is.EqualTo(Range.FromLength(val.Length, 0)), "#2");
				Assert.That(textArea.CaretIndex, Is.EqualTo(val.Length), "#3");
				Assert.That(textChanged, Is.EqualTo(1), "#4");
				Assert.That(selectionChanged, Is.EqualTo(1), "#5");

				textArea.Selection = Range.FromLength(6, 5);
				Assert.That(textArea.Selection, Is.EqualTo(Range.FromLength(6, 5)), "#6");
				Assert.That(textArea.CaretIndex, Is.EqualTo(6), "#7");
				Assert.That(textChanged, Is.EqualTo(1), "#8");
				Assert.That(selectionChanged, Is.EqualTo(2), "#9");

				textArea.Text = val = "Some other text";
				Assert.That(textArea.Selection, Is.EqualTo(Range.FromLength(val.Length, 0)), "#10");
				Assert.That(textArea.CaretIndex, Is.EqualTo(val.Length), "#11");
				Assert.That(textChanged, Is.EqualTo(2), "#12");
				Assert.That(selectionChanged, Is.EqualTo(3), "#13");
			});
		}

		[Test]
		public void EnabledShouldNotAffectReadOnly()
		{
			Invoke(() =>
			{
				var textArea = new T();
				Assert.That(textArea.Enabled, Is.True, "#1");
				Assert.That(textArea.ReadOnly, Is.False, "#2");
				textArea.Enabled = false;
				Assert.That(textArea.Enabled, Is.False, "#3");
				Assert.That(textArea.ReadOnly, Is.False, "#4");
				textArea.Enabled = true;
				Assert.That(textArea.Enabled, Is.True, "#5");
				Assert.That(textArea.ReadOnly, Is.False, "#6");
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
				Assert.That(textChangedCount, Is.EqualTo(1), "#1-1");
				Assert.That(textArea.Selection, Is.EqualTo(Range.FromLength(textArea.Text.TrimEnd().Length, 0)), "#1-2");
				Assert.That(selectionChangedCount, Is.EqualTo(1), "#1-3");

				textArea.Selection = Range.FromLength(6, 5);
				Assert.That(textChangedCount, Is.EqualTo(1), "#2-1");
				Assert.That(selectionChangedCount, Is.EqualTo(2), "#2-2");
				Assert.That(textArea.Selection, Is.EqualTo(Range.FromLength(6, 5)), "#2-3");

				return textArea;

			}, textArea =>
			{
				Assert.That(textChangedCount, Is.EqualTo(1), "#4-1");
				Assert.That(selectionChangedCount, Is.EqualTo(2), "#4-2");

				textArea.SelectedText = "my";
				Assert.That(textChangedCount, Is.EqualTo(2), "#5-1");
				Assert.That(selectionChangedCount, Is.EqualTo(3), "#5-2");
				Assert.That(textArea.Text.TrimEnd(), Is.EqualTo("Hello my friend"), "#5-3");
				Assert.That(textArea.Selection, Is.EqualTo(Range.FromLength(6, 2)), "#5-4");

				textArea.Selection = textArea.Selection.WithLength(textArea.Selection.Length() + 1);
				Assert.That(selectionChangedCount, Is.EqualTo(4), "#6");

				textArea.SelectedText = null;
				Assert.That(textChangedCount, Is.EqualTo(3), "#7-1");
				Assert.That(selectionChangedCount, Is.EqualTo(5), "#7-2");
				Assert.That(textArea.Text.TrimEnd(), Is.EqualTo("Hello friend"), "#7-3");
				Assert.That(textArea.Selection, Is.EqualTo(Range.FromLength(6, 0)), "#7-4");
			});
		}

		[Test]
		public void InitialValueOfSelectedTextShouldBeEmptyInsteadOfNull()
		{
			Invoke(() =>
			{
				var textArea = new T();
				Assert.That(textArea.SelectedText, Is.EqualTo(string.Empty), "SelectedText should be empty not null before setting any text");
				textArea.Text = "Hello!";
				Assert.That(textArea.SelectedText, Is.EqualTo(string.Empty), "SelectedText should *still* be empty not null after setting text");
			});
		}

		[Test]
		[ManualTest]
		public void ScaledShouldNotGrowDialog() => Invoke(() =>
		{
			var dlg = new Dialog();
			dlg.Content = new T() { Text = "Hello!  The dialog should not grow too large or anything, and should make this text wrap." };
			dlg.ShowModal();
		}, -1);
	}
}
