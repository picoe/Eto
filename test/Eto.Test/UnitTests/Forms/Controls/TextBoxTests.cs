using System;
using NUnit.Framework;
using Eto.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Eto.Test.UnitTests.Forms.Controls
{
	public abstract class TextBoxBase<T> : TestBase
		where T: TextBox, new()
	{
		[Test, ManualTest]
		public void CaretIndexShouldStartInInitialPosition()
		{
			ManualForm("Caret should be at index 2, between the 'e' and 'l'.", form =>
			{
				var textBox = new T();
				textBox.Text = "Hello";
				textBox.CaretIndex = 2;
				Assert.AreEqual(2, textBox.CaretIndex, "#1");
				Assert.AreEqual(new Range<int>(2, 1), textBox.Selection, "#2");
				return textBox;
			});
		}

		[Test]
		public void CaretIndexShouldRetainPositionOnInitialLoad()
		{
			Shown(form =>
			{
				var textBox = new T();
				textBox.Text = "Hello";
				textBox.CaretIndex = 2;
				Assert.AreEqual(2, textBox.CaretIndex, "#1");
				Assert.AreEqual(new Range<int>(2, 1), textBox.Selection, "#2");
				return textBox;
			}, textBox =>
			{
				Assert.AreEqual(2, textBox.CaretIndex, "#3");
				Assert.AreEqual(new Range<int>(2, 1), textBox.Selection, "#4");
			});
		}

		[Test, ManualTest]
		public void SelectionShouldStartInInitialPosition()
		{
			var text = "Hello";
			var selection = new Range<int>(1, text.Length - 2);
			ManualForm("Selection should be the entire string except the first and last characters.", form =>
			{
				var textBox = new T();
				textBox.Text = text;
				textBox.Selection = selection;
				Assert.AreEqual(selection, textBox.Selection, "#1");
				Assert.AreEqual(selection.Start, textBox.CaretIndex, "#2");
				return textBox;
			});
		}

		[Test]
		public void SelectionShouldRetainPositionOnInitialLoad()
		{
			var text = "Hello";
			var selection = new Range<int>(1, text.Length - 2);
			Shown(form =>
			{
				var textBox = new T();
				textBox.Text = text;
				textBox.Selection = selection;
				Assert.AreEqual(selection, textBox.Selection, "#1");
				Assert.AreEqual(selection.Start, textBox.CaretIndex, "#2");
				return textBox;
			}, textBox =>
			{
				Assert.AreEqual(selection, textBox.Selection, "#3");
				Assert.AreEqual(selection.Start, textBox.CaretIndex, "#4");
			});
		}

		[Test]
		public void SelectionShouldBeSetAfterFocus()
		{
			var text = "Hello";
			var selection = new Range<int>(1, text.Length - 2);
			T textBox = null;
			Shown(form =>
			{
				textBox = new T();
				textBox.Text = text;
				textBox.Selection = selection;
				Assert.AreEqual(selection, textBox.Selection, "#1");
				Assert.AreEqual(selection.Start, textBox.CaretIndex, "#2");
				form.Content = new TableLayout(
					new TextBox(),
					textBox
					);
			}, () =>
			{
				Assert.AreEqual(selection, textBox.Selection, "#3");
				Assert.AreEqual(selection.Start, textBox.CaretIndex, "#4");
				textBox.Focus();
				Assert.AreEqual(selection, textBox.Selection, "#5");
				Assert.AreEqual(selection.Start, textBox.CaretIndex, "#6");
			});
		}

		[Test]
		public void CaretIndexShouldUpdateSelection()
		{
			var text = "Hello";
			var selection = new Range<int>(1, text.Length - 2);
			Shown(form =>
			{
				var textBox = new T();
				textBox.Text = text;
				textBox.Selection = selection;
				Assert.AreEqual(selection, textBox.Selection, "#1");
				Assert.AreEqual(selection.Start, textBox.CaretIndex, "#2");
				textBox.CaretIndex = 2;
				Assert.AreEqual(new Range<int>(2, 1), textBox.Selection, "#3");
				Assert.AreEqual(2, textBox.CaretIndex, "#4");
				return textBox;
			}, textBox =>
			{
				Assert.AreEqual(new Range<int>(2, 1), textBox.Selection, "#5");
				Assert.AreEqual(2, textBox.CaretIndex, "#6");
				textBox.Selection = selection;
				Assert.AreEqual(selection, textBox.Selection, "#7");
				Assert.AreEqual(selection.Start, textBox.CaretIndex, "#8");
			});
		}

		[TestCaseSource(typeof(TextChangingEventArgsTests), nameof(TextChangingEventArgsTests.GetTextChangingCases))]
		public void TextChangingShouldReturnCorrectResults(string oldText, string newText, string text, int rangeStart, int rangeLength)
		{
			Invoke(() =>
			{
				TextChangingEventArgs args = null;
				var tb = new TextBox();
				tb.Text = oldText;

				tb.TextChanging += (sender, e) => args = e;
				tb.Text = newText;

				Assert.IsNotNull(args, "#1");
				Assert.AreEqual(oldText ?? string.Empty, args.OldText, "#2");
				Assert.AreEqual(newText ?? string.Empty, args.NewText, "#3");
				Assert.AreEqual(text, args.Text, "#4");
				Assert.AreEqual(Range.FromLength(rangeStart, rangeLength), args.Range, "#5");

			});
		}

		[ManualTest]
		[TestCaseSource(typeof(TextChangingEventArgsTests), nameof(TextChangingEventArgsTests.GetTextChangingCases))]
		public void InsertingTextShouldFireTextChanging(string oldText, string newText, string text, int rangeStart, int rangeLength)
		{
			TextChangingEventArgs args = null;
			Form(form =>
			{
				var textToSelect = oldText?.Substring(rangeStart, rangeLength) ?? string.Empty;
				var tb = new TextBox
				{
					AutoSelectMode = AutoSelectMode.Never,
					Text = oldText,
					Selection = Range.FromLength(rangeStart, rangeLength)
				};
				tb.TextChanging += (sender, e) =>
				{
					args = e;
					form.Close();
				};
				tb.Focus();

				Assert.AreEqual(textToSelect, tb.SelectedText, "#1");

				new Clipboard().Text = text;

				var help = new Label
				{
					Text = $"Select '{textToSelect}', and paste '{text}' (which should be on the clipboard)"
				};

				form.Content = new StackLayout
				{
					Padding = 10,
					Spacing = 10,
					Items = { help, tb }
				};
			}, -1);

			Assert.IsNotNull(args, "#2.1");
			Assert.AreEqual(oldText ?? string.Empty, args.OldText, "#2.2");
			Assert.AreEqual(newText ?? string.Empty, args.NewText, "#2.3");
			Assert.AreEqual(text, args.Text, "#2.4");
			Assert.AreEqual(Range.FromLength(rangeStart, rangeLength), args.Range, "#2.5");
		}
	}


	[TestFixture]
	public class TextBoxTests : TextBoxBase<TextBox>
	{
	}

	[TestFixture]
	public class TextStepperTests : TextBoxBase<TextStepper>
	{
	}
}
