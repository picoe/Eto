using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using System;

namespace Eto.Test.Sections.Drawing
{
    [Section("Drawing", typeof(FormattedText))]
    public class FormattedTextSection : Scrollable
    {
        public FormattedTextSection()
        {
            var font = SystemFonts.Default();
            var color = Colors.White;

            var formattedText = new FormattedText
            {
                Text = Sections.Controls.RichTextAreaSection.LoremText,
                MaximumSize = new SizeF(200, 100),
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

                var location = new Point(10, 10);

                g.DrawText(font, color, location, "Single Line Text That Will Not Wrap 漢字");

                location.Y += 40;

				g.DrawRectangle(Colors.Blue, new RectangleF(location, formattedText.MaximumSize));

				var size = formattedText.Measure();
                g.DrawText(formattedText, location);
                g.DrawRectangle(Colors.Silver, new RectangleF(location, size));
            };

            var wrapMode = new EnumDropDown<FormattedTextWrapMode>();
            wrapMode.SelectedValueBinding.Bind(formattedText, f => f.Wrap);
            wrapMode.SelectedValueChanged += (sender, e) => control.Invalidate();

            var trimming = new EnumDropDown<FormattedTextTrimming>();
            trimming.SelectedValueBinding.Bind(formattedText, f => f.Trimming);
            trimming.SelectedValueChanged += (sender, e) => control.Invalidate();

            var alignment = new EnumDropDown<FormattedTextAlignment>();
            alignment.SelectedValueBinding.Bind(formattedText, f => f.Alignment);
            alignment.SelectedValueChanged += (sender, e) => control.Invalidate();

			var fontSelection = new FontPicker();
			fontSelection.ValueBinding.Bind(formattedText, f => f.Font);
			fontSelection.ValueChanged += (sender, e) => control.Invalidate();

			var maxSizeEntry = new SizeFEntry();
			maxSizeEntry.ValueBinding.Bind(formattedText, f => f.MaximumSize);
			maxSizeEntry.ValueChanged += (sender, e) => control.Invalidate();

			var layout = new DynamicLayout();

            layout.BeginCentered();

            layout.AddSeparateRow("Wrap:", wrapMode, null);
            layout.AddSeparateRow("Trimming:", trimming, null);
			layout.AddSeparateRow("Alignment:", alignment, null);
			layout.AddSeparateRow("Font:", fontSelection, null);
			layout.AddSeparateRow("MaximumSize:", maxSizeEntry, null);

			

            layout.EndCentered();

            layout.Add(control);
            Content = layout;
        }
    }
}
