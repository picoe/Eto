using Eto.Test.UnitTests;
using Eto.Wpf.Forms.Controls;
using NUnit.Framework;
using swm = System.Windows.Media;
using swmi = System.Windows.Media.Imaging;

namespace Eto.Test.Wpf.UnitTests
{
	/// <summary>
	/// WPF is the backend that can both wrap and trim, so it is where the two properties are checked
	/// against what the native control ends up doing.
	/// </summary>
	[TestFixture]
	public class TextTrimmingTests : TestBase
	{
		const string LongText = "https://www.example.com/a/rather/long/path/that/will/not/fit/in/a/narrow/control";

		// Word wrap cannot break inside a word, so the comparison against a wrapped control needs text with spaces in it.
		const string LongSentence = "The quick brown fox jumps over the lazy dog and keeps on running for quite a while";

		static sw.TextTrimming NativeTrimming(Label label)
		{
			var native = (EtoAccessLabel)label.ControlObject;
			return native.Content switch
			{
				swc.TextBlock textBlock => textBlock.TextTrimming,
				swc.AccessText accessText => accessText.TextTrimming,
				_ => sw.TextTrimming.None
			};
		}

		[TestCase(TextTrimming.None, sw.TextTrimming.None)]
		[TestCase(TextTrimming.CharacterEllipsis, sw.TextTrimming.CharacterEllipsis)]
		[TestCase(TextTrimming.WordEllipsis, sw.TextTrimming.WordEllipsis)]
		public void LabelShouldSetNativeTrimming(TextTrimming trimming, sw.TextTrimming expected) => Invoke(() =>
		{
			var label = new Label { Text = LongText, Trimming = trimming };
			Assert.That(NativeTrimming(label), Is.EqualTo(expected));
		});

		/// <summary>
		/// The content is rebuilt whenever the text changes, so the trimming has to survive that.
		/// </summary>
		[Test]
		public void LabelShouldKeepTrimmingWhenTextChanges() => Invoke(() =>
		{
			var label = new Label { Text = "short", Trimming = TextTrimming.CharacterEllipsis };
			label.Text = LongText;
			Assert.That(NativeTrimming(label), Is.EqualTo(sw.TextTrimming.CharacterEllipsis));
		});

		[TestCase(TextTrimming.None, sw.TextTrimming.None)]
		[TestCase(TextTrimming.CharacterEllipsis, sw.TextTrimming.CharacterEllipsis)]
		[TestCase(TextTrimming.WordEllipsis, sw.TextTrimming.WordEllipsis)]
		public void LinkButtonShouldSetNativeTrimming(TextTrimming trimming, sw.TextTrimming expected) => Invoke(() =>
		{
			var link = new LinkButton { Text = LongText, Trimming = trimming };
			Assert.That(((swc.TextBlock)link.ControlObject).TextTrimming, Is.EqualTo(expected));
		});


		/// <summary>
		/// A link's colour lives on its Hyperlink inline, but the trimming ellipsis is drawn by the
		/// TextBlock around it from its own properties - so the TextBlock has to be given the same colours
		/// or the ellipsis comes out in the default text colour beside link-coloured text.
		/// </summary>
		[TestCase(false)]
		[TestCase(true)]
		public void TrimmedLinkEllipsisShouldUseTheLinkColour(bool disabled)
		{
			var expected = disabled ? Colors.Lime : Colors.Red;
			LinkButton link = null;
			Shown(form =>
			{
				link = new LinkButton
				{
					Text = LongText,
					Width = 120,
					Wrap = WrapMode.None,
					Trimming = TextTrimming.CharacterEllipsis,
					TextColor = Colors.Red,
					DisabledTextColor = Colors.Lime,
					Enabled = !disabled
				};
				form.Content = new StackLayout { HorizontalContentAlignment = HorizontalAlignment.Left, Items = { link } };
			}, () =>
			{
				var textBlock = (swc.TextBlock)link.ControlObject;

				// There has to actually be an ellipsis, or the colour check below proves nothing.
				link.Trimming = TextTrimming.None;
				textBlock.UpdateLayout();
				var untrimmed = OpaqueCount(textBlock);
				link.Trimming = TextTrimming.CharacterEllipsis;
				textBlock.UpdateLayout();
				Assert.That(OpaqueCount(textBlock), Is.Not.EqualTo(untrimmed), "Text was not trimmed, so there is no ellipsis to check the colour of");

				var expectedHex = $"#{expected.Rb / 64 * 64:X2}{expected.Gb / 64 * 64:X2}{expected.Bb / 64 * 64:X2}";
				Assert.That(DrawnColours(textBlock), Is.EqualTo(new[] { expectedHex }));
			});
		}

		/// <summary>
		/// How many fully opaque pixels an element renders, used only to tell that the text really was
		/// trimmed - the ellipsis itself is never opaque, which is the trap this file fell into once.
		/// </summary>
		static int OpaqueCount(sw.FrameworkElement element)
		{
			var width = (int)Math.Ceiling(element.ActualWidth);
			var height = (int)Math.Ceiling(element.ActualHeight);
			var bitmap = new swmi.RenderTargetBitmap(width, height, 96, 96, swm.PixelFormats.Pbgra32);
			bitmap.Render(element);
			var stride = width * 4;
			var pixels = new byte[stride * height];
			bitmap.CopyPixels(pixels, stride, 0);
			var count = 0;
			for (var i = 3; i < pixels.Length; i += 4)
				if (pixels[i] >= 250)
					count++;
			return count;
		}

		static List<string> DrawnColours(sw.FrameworkElement element)
		{
			var width = (int)Math.Ceiling(element.ActualWidth);
			var height = (int)Math.Ceiling(element.ActualHeight);
			var bitmap = new swmi.RenderTargetBitmap(width, height, 96, 96, swm.PixelFormats.Pbgra32);
			bitmap.Render(element);

			var stride = width * 4;
			var pixels = new byte[stride * height];
			bitmap.CopyPixels(pixels, stride, 0);

			var colours = new List<string>();
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var i = (y * stride) + (x * 4);
					var a = pixels[i + 3];
					// Ignore the faintest anti-aliasing only.  The ellipsis is three small dots that never
					// reach full opacity, so a stricter threshold skips the very thing under test.
					if (a < 60)
						continue;

					// Pbgra32 is premultiplied, so recover the colour the glyph was drawn in - otherwise a
					// half-transparent red pixel reads as dark red and is indistinguishable from grey.
					var b = Math.Min(255, pixels[i] * 255 / a);
					var g = Math.Min(255, pixels[i + 1] * 255 / a);
					var r = Math.Min(255, pixels[i + 2] * 255 / a);

					// Quantise so anti-aliasing noise does not read as a different colour.
					var colour = $"#{r / 64 * 64:X2}{g / 64 * 64:X2}{b / 64 * 64:X2}";
					if (!colours.Contains(colour))
						colours.Add(colour);
				}
			}
			return colours;
		}
	}
}
