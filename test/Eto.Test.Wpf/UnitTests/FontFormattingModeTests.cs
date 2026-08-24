using Eto.Test.UnitTests;
using Eto.Wpf.Drawing;
using NUnit.Framework;
using swd = System.Windows.Documents;
using swm = System.Windows.Media;

namespace Eto.Test.Wpf.UnitTests
{
	/// <summary>
	/// Tests for laying out text with a specific <see cref="swm.TextFormattingMode"/>.
	/// </summary>
	/// <remarks>
	/// WPF lays out text using the mode of the element it is rendered in, so text that Eto measures or draws
	/// itself has to be told which mode to use, otherwise measuring reports a size the text isn't rendered with.
	/// The difference adds up per glyph, so it is most visible with long text in a small font at 96dpi.
	/// </remarks>
	[TestFixture]
	public class FontFormattingModeTests : TestBase
	{
		const string LongText = "RenderWindowCopyToClipboardAndThenSomeMore";

		static Font CreateFont() => new Font("Segoe UI", 8.25f);

		static FontHandler GetHandler(Font font) => (FontHandler)font.Handler;

		/// <summary>
		/// Lays out the text with WPF directly to compare what Eto reports against.
		/// </summary>
		static double ReferenceWidth(Font font, swm.TextFormattingMode mode, double pixelsPerDip, string text = LongText)
		{
			var handler = GetHandler(font);
			var formattedText = new swm.FormattedText(
				text,
				CultureInfo.CurrentUICulture,
				sw.FlowDirection.LeftToRight,
				handler.WpfTypeface,
				handler.WpfSize,
				swm.Brushes.Black,
				null,
				mode,
				pixelsPerDip);
			return formattedText.WidthIncludingTrailingWhitespace;
		}

		[Test]
		public void FontShouldNotSpecifyFormattingModeByDefault()
		{
			Invoke(() =>
			{
				var font = CreateFont();
				var handler = GetHandler(font);
				Assert.That(handler.TextFormattingMode, Is.Null, "#1 the mode of what the text is rendered in should be used by default");
				Assert.That(font.MeasureString(LongText).Width, Is.EqualTo(ReferenceWidth(font, swm.TextFormattingMode.Ideal, handler.PixelsPerDip)).Within(0.01), "#2 should measure with ideal layout by default");
			});
		}

		[Test]
		public void MeasureStringShouldUseSpecifiedFormattingMode()
		{
			Invoke(() =>
			{
				var font = CreateFont();
				var handler = GetHandler(font);
				handler.TextFormattingMode = swm.TextFormattingMode.Display;
				handler.PixelsPerDip = 1; // 96dpi, where display layout snaps each glyph to a whole dip

				var display = ReferenceWidth(font, swm.TextFormattingMode.Display, 1);
				Assume.That(display, Is.Not.EqualTo(ReferenceWidth(font, swm.TextFormattingMode.Ideal, 1)), "the two modes should lay this text out differently, otherwise this proves nothing");

				Assert.That(font.MeasureString(LongText).Width, Is.EqualTo(display).Within(0.01));
			});
		}

		[TestCase(swm.TextFormattingMode.Ideal)]
		[TestCase(swm.TextFormattingMode.Display)]
		public void MeasureStringShouldMatchWhatWpfLaysOut(swm.TextFormattingMode mode)
		{
			Invoke(() =>
			{
				var font = CreateFont();
				var handler = GetHandler(font);
				handler.TextFormattingMode = mode;
				// note: leave PixelsPerDip alone, a standalone element is measured for the system dpi

				// measure the text the way WPF does when it renders it in an element using that mode
				var textBlock = new swc.TextBlock { Text = LongText };
				handler.Apply(textBlock, null);
				textBlock.Measure(new sw.Size(double.PositiveInfinity, double.PositiveInfinity));

				Assert.That(font.MeasureString(LongText).Width, Is.EqualTo(textBlock.DesiredSize.Width).Within(1.0));
			});
		}

		[Test]
		public void PixelsPerDipShouldNotScaleMeasuredSize()
		{
			Invoke(() =>
			{
				var font = CreateFont();
				var handler = GetHandler(font);

				// ideal layout doesn't snap glyphs to device pixels, so the dpi makes no difference at all..
				handler.PixelsPerDip = 1;
				var idealAt96 = font.MeasureString(LongText);
				handler.PixelsPerDip = 2;
				Assert.That(font.MeasureString(LongText), Is.EqualTo(idealAt96), "#1 ideal layout should not depend on the dpi");

				// ..display layout does, but the size is still in dips rather than scaled by it
				handler.TextFormattingMode = swm.TextFormattingMode.Display;
				handler.PixelsPerDip = 1;
				var displayAt96 = font.MeasureString(LongText).Width;
				handler.PixelsPerDip = 2;
				var displayAt192 = font.MeasureString(LongText).Width;
				Assume.That(displayAt192, Is.Not.EqualTo(displayAt96), "display layout should depend on the dpi for this text, otherwise this proves nothing");
				Assert.That(displayAt192, Is.EqualTo(displayAt96).Within(idealAt96.Width / 4), "#2 the size should be in dips, not scaled by the dpi");
			});
		}

		[Test]
		public void StyleShouldSetFormattingModeOnNewFonts()
		{
			Invoke(() =>
			{
				var provider = new DefaultStyleProvider();
				provider.Add<FontHandler>(null, h => h.TextFormattingMode = swm.TextFormattingMode.Display);

				var oldProvider = Style.Provider;
				Style.Provider = provider;
				try
				{
					Assert.That(GetHandler(CreateFont()).TextFormattingMode, Is.EqualTo(swm.TextFormattingMode.Display));
				}
				finally
				{
					Style.Provider = oldProvider;
				}
			});
		}

		[Test]
		public void ApplyingFontShouldSetFormattingMode()
		{
			Invoke(() =>
			{
				var font = CreateFont();
				var handler = GetHandler(font);
				handler.TextFormattingMode = swm.TextFormattingMode.Display;

				var control = new swc.TextBox();
				handler.Apply(control, null);
				Assert.That(swm.TextOptions.GetTextFormattingMode(control), Is.EqualTo(swm.TextFormattingMode.Display), "#1 control");

				var textBlock = new swc.TextBlock();
				handler.Apply(textBlock, null);
				Assert.That(swm.TextOptions.GetTextFormattingMode(textBlock), Is.EqualTo(swm.TextFormattingMode.Display), "#2 text block");

				var run = new swd.Run();
				handler.Apply(run, null);
				Assert.That(swm.TextOptions.GetTextFormattingMode(run), Is.EqualTo(swm.TextFormattingMode.Display), "#3 text element");
			});
		}

		[Test]
		public void ApplyingFontShouldNotSetFormattingModeWhenNotSpecified()
		{
			Invoke(() =>
			{
				var font = CreateFont();
				var handler = GetHandler(font);
				Assume.That(handler.TextFormattingMode, Is.Null);

				var control = new swc.TextBox();
				handler.Apply(control, null);
				Assert.That(control.ReadLocalValue(swm.TextOptions.TextFormattingModeProperty), Is.EqualTo(sw.DependencyProperty.UnsetValue), "#1 should keep inheriting the mode of its container");

				var label = new Label { Text = LongText, Font = font };
				var labelControl = ((Eto.Wpf.Forms.Controls.LabelHandler)label.Handler).Control;
				Assert.That(labelControl.ReadLocalValue(swm.TextOptions.TextFormattingModeProperty), Is.EqualTo(sw.DependencyProperty.UnsetValue), "#2 a control with a font should keep inheriting it too");
			});
		}

		[Test]
		public void SettingControlFontShouldSetFormattingMode()
		{
			Invoke(() =>
			{
				var font = CreateFont();
				GetHandler(font).TextFormattingMode = swm.TextFormattingMode.Display;

				var label = new Label { Text = LongText, Font = font };
				var labelControl = ((Eto.Wpf.Forms.Controls.LabelHandler)label.Handler).Control;
				Assert.That(swm.TextOptions.GetTextFormattingMode(labelControl), Is.EqualTo(swm.TextFormattingMode.Display), "the control should render the text the same way it is measured");
			});
		}

		[Test]
		public void GraphicsShouldUseFontFormattingMode()
		{
			Invoke(() =>
			{
				var font = CreateFont();
				var handler = GetHandler(font);
				handler.TextFormattingMode = swm.TextFormattingMode.Display;
				handler.PixelsPerDip = 3; // should be ignored, the dpi of what we draw on is known

				using (var bitmap = new Bitmap(400, 50, PixelFormat.Format32bppRgba))
				using (var graphics = new Graphics(bitmap))
				{
					// a bitmap is its own 96dpi device, no matter what the dpi of the system or screen is
					var expected = ReferenceWidth(font, swm.TextFormattingMode.Display, 1);
					Assume.That(expected, Is.Not.EqualTo(ReferenceWidth(font, swm.TextFormattingMode.Display, 3)), "the dpi should matter for this text, otherwise this proves nothing");

					Assert.That(graphics.MeasureString(font, LongText).Width, Is.EqualTo(expected).Within(0.01), "#1 should measure for what it is drawn on");
					Assert.DoesNotThrow(() => graphics.DrawText(font, Colors.Black, 0, 0, LongText), "#2");
				}
			});
		}

		[Test]
		public void GraphicsShouldUseDpiOfWhatIsDrawnOn()
		{
			Paint((drawable, e) =>
			{
				var font = CreateFont();
				var handler = GetHandler(font);
				handler.TextFormattingMode = swm.TextFormattingMode.Display;
				handler.PixelsPerDip = 3; // should be ignored, the dpi of the drawable is known

				var pixelsPerDip = ((GraphicsHandler)e.Graphics.Handler).DPI;
				var expected = ReferenceWidth(font, swm.TextFormattingMode.Display, pixelsPerDip);
				Assume.That(expected, Is.Not.EqualTo(ReferenceWidth(font, swm.TextFormattingMode.Display, 3)), "the dpi should matter for this text, otherwise this proves nothing");

				Assert.That(e.Graphics.MeasureString(font, LongText).Width, Is.EqualTo(expected).Within(0.01));
			});
		}

		[Test]
		public void FormattedTextShouldUseFontFormattingMode()
		{
			Invoke(() =>
			{
				var font = CreateFont();
				var handler = GetHandler(font);
				handler.TextFormattingMode = swm.TextFormattingMode.Display;
				handler.PixelsPerDip = 1;

				var formattedText = new FormattedText { Font = font, Text = LongText, Wrap = FormattedTextWrapMode.None };
				Assert.That(formattedText.Measure().Width, Is.EqualTo(ReferenceWidth(font, swm.TextFormattingMode.Display, 1)).Within(0.01));
			});
		}

		[Test]
		public void ChangingFontShouldUpdateFormattedTextFormattingMode()
		{
			Invoke(() =>
			{
				var idealFont = CreateFont();
				GetHandler(idealFont).PixelsPerDip = 1;

				var displayFont = CreateFont();
				var displayHandler = GetHandler(displayFont);
				displayHandler.TextFormattingMode = swm.TextFormattingMode.Display;
				displayHandler.PixelsPerDip = 1;

				var expectedIdeal = ReferenceWidth(idealFont, swm.TextFormattingMode.Ideal, 1);
				var expectedDisplay = ReferenceWidth(displayFont, swm.TextFormattingMode.Display, 1);
				Assume.That(expectedDisplay, Is.Not.EqualTo(expectedIdeal), "the two modes should lay this text out differently, otherwise this proves nothing");

				var formattedText = new FormattedText { Font = idealFont, Text = LongText, Wrap = FormattedTextWrapMode.None };
				Assert.That(formattedText.Measure().Width, Is.EqualTo(expectedIdeal).Within(0.01), "#1");

				// the mode can only be specified when the text is created, so it has to be recreated
				formattedText.Font = displayFont;
				Assert.That(formattedText.Measure().Width, Is.EqualTo(expectedDisplay).Within(0.01), "#2 should use the mode of the new font");
			});
		}
	}
}
