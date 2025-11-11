using NUnit.Framework;
using Range = Eto.Forms.Range;

namespace Eto.Test.UnitTests.Forms.Controls
{
	public abstract class TextBoxBase<T> : TestBase
		where T : TextBox, new()
	{
		[Test, ManualTest]
		public void SettingTextShouldClearUndoBuffer()
		{
			ManualForm("Try typing and undo/redo, then press the button to reset. After reset, it should not undo to previous values", form =>
			{
				var textBox = new T();
				textBox.Text = "Hello";
				textBox.SelectAll();

				var button = new Button { Text = "Click Me" };
				button.Click += (sender, e) =>
				{
					textBox.Text = "Thanks, now try to undo";
					textBox.Focus();
				};

				return new TableLayout
				{
					Spacing = new Size(5, 5),
					Padding = 10,
					Rows = {
						textBox,
						button
					}
				};
			});
		}

		[Test, ManualTest]
		public void CaretIndexShouldStartInInitialPosition()
		{
			ManualForm("Caret should be at index 2, between the 'e' and 'l'.", form =>
			{
				var textBox = new T();
				textBox.Text = "Hello";
				textBox.CaretIndex = 2;
				Assert.That(textBox.CaretIndex, Is.EqualTo(2), "#1");
				Assert.That(textBox.Selection, Is.EqualTo(new Range<int>(2, 1)), "#2");
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
				Assert.That(textBox.CaretIndex, Is.EqualTo(2), "#1");
				Assert.That(textBox.Selection, Is.EqualTo(new Range<int>(2, 1)), "#2");
				return textBox;
			}, textBox =>
			{
				Assert.That(textBox.CaretIndex, Is.EqualTo(2), "#3");
				Assert.That(textBox.Selection, Is.EqualTo(new Range<int>(2, 1)), "#4");
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
				Assert.That(textBox.Selection, Is.EqualTo(selection), "#1");
				Assert.That(textBox.CaretIndex, Is.EqualTo(selection.Start), "#2");
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
				Assert.That(textBox.Selection, Is.EqualTo(selection), "#1");
				Assert.That(textBox.CaretIndex, Is.EqualTo(selection.Start), "#2");
				return textBox;
			}, textBox =>
			{
				Assert.That(textBox.Selection, Is.EqualTo(selection), "#3");
				Assert.That(textBox.CaretIndex, Is.EqualTo(selection.Start), "#4");
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
				Assert.That(textBox.Selection, Is.EqualTo(selection), "#1");
				Assert.That(textBox.CaretIndex, Is.EqualTo(selection.Start), "#2");
				form.Content = new TableLayout(
					new TextBox(),
					textBox
					);
			}, () =>
			{
				Assert.That(textBox.Selection, Is.EqualTo(selection), "#3");
				Assert.That(textBox.CaretIndex, Is.EqualTo(selection.Start), "#4");
				textBox.Focus();
				Assert.That(textBox.Selection, Is.EqualTo(selection), "#5");
				Assert.That(textBox.CaretIndex, Is.EqualTo(selection.Start), "#6");
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
				Assert.That(textBox.Selection, Is.EqualTo(selection), "#1");
				Assert.That(textBox.CaretIndex, Is.EqualTo(selection.Start), "#2");
				textBox.CaretIndex = 2;
				Assert.That(textBox.Selection, Is.EqualTo(new Range<int>(2, 1)), "#3");
				Assert.That(textBox.CaretIndex, Is.EqualTo(2), "#4");
				return textBox;
			}, textBox =>
			{
				Assert.That(textBox.Selection, Is.EqualTo(new Range<int>(2, 1)), "#5");
				Assert.That(textBox.CaretIndex, Is.EqualTo(2), "#6");
				textBox.Selection = selection;
				Assert.That(textBox.Selection, Is.EqualTo(selection), "#7");
				Assert.That(textBox.CaretIndex, Is.EqualTo(selection.Start), "#8");
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

				Assert.That(args, Is.Not.Null, "#1");
				Assert.That(args.OldText, Is.EqualTo(oldText ?? string.Empty), "#2");
				Assert.That(args.NewText, Is.EqualTo(newText ?? string.Empty), "#3");
				Assert.That(args.Text, Is.EqualTo(text), "#4");
				Assert.That(args.Range, Is.EqualTo(Range.FromLength(rangeStart, rangeLength)), "#5");

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

				Assert.That(tb.SelectedText, Is.EqualTo(textToSelect), "#1");

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

			Assert.That(args, Is.Not.Null, "#2.1");
			Assert.That(args.OldText, Is.EqualTo(oldText ?? string.Empty), "#2.2");
			Assert.That(args.NewText, Is.EqualTo(newText ?? string.Empty), "#2.3");
			Assert.That(args.Text, Is.EqualTo(text), "#2.4");
			Assert.That(args.Range, Is.EqualTo(Range.FromLength(rangeStart, rangeLength)), "#2.5");
		}

		[Test, ManualTest]
		public void ManyUpdatesShouldNotCauseHangs()
		{
			TimeSpan maxElapsed = TimeSpan.MinValue;
			ManualForm(
			"There should not be any pausing",
			form =>
			{
				var textBoxes = new List<T>();
				var layout = new DynamicLayout();
				for (int x = 0; x < 10; x++)
				{
					layout.BeginHorizontal();
					for (int y = 0; y < 10; y++)
					{
						var textBox = new T();
						textBoxes.Add(textBox);
						layout.Add(textBox, true);
					}
					layout.EndHorizontal();
				}
				var sw = new Stopwatch();
				var timer = new UITimer { Interval = 0.01 };
				timer.Elapsed += (sender, e) =>
				{
					var elapsed = sw.Elapsed;
					if (elapsed > maxElapsed)
					{
						maxElapsed = elapsed;
					}
					sw.Restart();
					var rnd = new Random();
					foreach (var tb in textBoxes)
					{
						tb.Text = rnd.Next(int.MaxValue).ToString();
					}
				};
				form.Shown += (sender, e) =>
				{
					timer.Start();
					sw.Start();
				};
				form.Closed += (sender, e) => timer.Stop();

				return layout;
			});
			Assert.That(maxElapsed, Is.LessThan(TimeSpan.FromSeconds(1)), "There were long pauses in the UI");
		}
		
		[TestCase(false)]
		[TestCase(true)]
		public void TextChangedShouldFireAfterSelectionWasChangedForNeverAutoSelectMode(bool withFocus)
		{
			bool textChangedFired = false;
			Range<int>? selection = null;
			string newText = null;
			Shown(form =>
			{
				var textBox = new T();
				textBox.AutoSelectMode = AutoSelectMode.Never;
				textBox.Text = "Hello";
				textBox.Selection = new Range<int>(1, 3);
				textBox.TextChanged += (sender, e) =>
				{
					textChangedFired = true;
					selection = textBox.Selection;
					newText = textBox.Text;
				};
				
				return textBox;
			}, textBox =>
			{
				if (withFocus)
					textBox.Focus();

				Assert.That(textChangedFired, Is.False, "#1");
				var setText = "Something else";
				textBox.Text = setText;
				Assert.That(textChangedFired, Is.True, "#2");
				Assert.That(selection, Is.EqualTo(Range.FromLength(setText.Length, 0)), "#3");
				Assert.That(newText, Is.EqualTo(setText), "#4");
				Assert.That(textBox.Text, Is.EqualTo(setText), "#5");
				Assert.That(textBox.Selection, Is.EqualTo(Range.FromLength(setText.Length, 0)), "#6");
			});
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
