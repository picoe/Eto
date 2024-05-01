namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", typeof(FormattedText))]
	public class FormattedTextSection : Scrollable
	{
		public FormattedTextSection()
		{
			Styles.Add<Label>(null, l => l.VerticalAlignment = VerticalAlignment.Center);
			
			var font = SystemFonts.Default();
			var color = Colors.White;

			var formattedTextSingleLine = new FormattedText
			{
				Text = "Some text with no new lines that should have an ellipsis when not wrapped",
				MaximumSize = new SizeF(500, float.MaxValue),
				Wrap = FormattedTextWrapMode.Word,
				Trimming = FormattedTextTrimming.CharacterEllipsis,
				ForegroundBrush = Brushes.White,
				//ForegroundBrush = new LinearGradientBrush(Colors.White, Colors.Blue, new PointF(0, 0), new PointF(200, 200)),
				Font = font
			};

			var formattedText = new FormattedText
			{
				Text = Utility.LoremTextWithTwoParagraphs,
				MaximumSize = new SizeF(500, 80),
				Wrap = FormattedTextWrapMode.Word,
				Trimming = FormattedTextTrimming.CharacterEllipsis,
				ForegroundBrush = Brushes.White,
				//ForegroundBrush = new LinearGradientBrush(Colors.White, Colors.Blue, new PointF(0, 0), new PointF(200, 200)),
				Font = font
			};

			var formattedTextWithNewLines = new FormattedText
			{
				Text = Utility.LoremTextWithNewLines,
				Wrap = FormattedTextWrapMode.Word,
				Trimming = FormattedTextTrimming.CharacterEllipsis,
				ForegroundBrush = Brushes.White,
				//ForegroundBrush = new LinearGradientBrush(Colors.White, Colors.Blue, new PointF(0, 0), new PointF(200, 200)),
				Font = font
			};

			var control = new Drawable { Size = new Size(400, 500), BackgroundColor = Colors.Black };
			control.Paint += (sender, e) =>
			{
				var g = e.Graphics;

				var location = new PointF(10, 10);

				g.DrawText(font, color, location, "Single Line Text That Will Not Wrap 漢字");

				location.Y += 40;
				
				var rect = new RectangleF(location, new SizeF(300, 20));
				g.DrawRectangle(Colors.Blue, rect);
				g.DrawText(font, new SolidBrush(color), rect, "Should Be Right Aligned", FormattedTextWrapMode.None, FormattedTextAlignment.Right, FormattedTextTrimming.None);

				location.Y += 40;

				var sizeSingleLine = formattedTextSingleLine.Measure();
				g.DrawText(formattedTextSingleLine, location);
				g.DrawRectangle(Colors.Silver, new RectangleF(location, sizeSingleLine));

				location.Y += sizeSingleLine.Height + 20;

				g.DrawRectangle(Colors.Blue, new RectangleF(location, formattedText.MaximumSize));
				var size = formattedText.Measure();
				g.DrawText(formattedText, location);
				g.DrawRectangle(Colors.Silver, new RectangleF(location, size));

				location.Y += (int)formattedText.MaximumHeight + 20;

				var sizeWithNewLines = formattedTextWithNewLines.Measure();
				g.DrawText(formattedTextWithNewLines, location);
				g.DrawRectangle(Colors.Silver, new RectangleF(location, sizeWithNewLines));
				
			};

			var wrapMode = new EnumDropDown<FormattedTextWrapMode>();
			wrapMode.SelectedValueBinding.Bind(formattedText, f => f.Wrap);
			wrapMode.SelectedValueBinding.Bind(formattedTextWithNewLines, f => f.Wrap);
			wrapMode.SelectedValueBinding.Bind(formattedTextSingleLine, f => f.Wrap);
			wrapMode.SelectedValueChanged += (sender, e) => control.Invalidate();

			var trimming = new EnumDropDown<FormattedTextTrimming>();
			trimming.SelectedValueBinding.Bind(formattedText, f => f.Trimming);
			trimming.SelectedValueBinding.Bind(formattedTextWithNewLines, f => f.Trimming);
			trimming.SelectedValueBinding.Bind(formattedTextSingleLine, f => f.Trimming);
			trimming.SelectedValueChanged += (sender, e) => control.Invalidate();

			var alignment = new EnumDropDown<FormattedTextAlignment>();
			alignment.SelectedValueBinding.Bind(formattedText, f => f.Alignment);
			alignment.SelectedValueBinding.Bind(formattedTextWithNewLines, f => f.Alignment);
			alignment.SelectedValueBinding.Bind(formattedTextSingleLine, f => f.Alignment);
			alignment.SelectedValueChanged += (sender, e) => control.Invalidate();

			var fontSelection = new FontPicker();
			fontSelection.ValueBinding.Bind(formattedText, f => f.Font);
			fontSelection.ValueBinding.Bind(formattedTextWithNewLines, f => f.Font);
			fontSelection.ValueBinding.Bind(formattedTextSingleLine, f => f.Font);
			fontSelection.ValueChanged += (sender, e) => control.Invalidate();

			var maxSizeEntry = new SizeFEntry();
			maxSizeEntry.ValueBinding.Bind(formattedText, f => f.MaximumSize);
			maxSizeEntry.ValueBinding.Child(r => r.Width).Bind(formattedTextSingleLine, f => (int)f.MaximumSize.Width);
			maxSizeEntry.ValueChanged += (sender, e) => control.Invalidate();

			var layout = new DynamicLayout();
			layout.DefaultSpacing = new Size(4, 4);

			layout.BeginCentered();

			layout.AddSeparateRow(
				"Wrap:", wrapMode,
				"Trimming:", trimming,
				"Alignment:", alignment,
				"Font:", fontSelection,
				null
				);
			layout.AddSeparateRow("MaximumSize:", maxSizeEntry, null);



			layout.EndCentered();

			layout.Add(control);
			Content = layout;
		}
	}
}
