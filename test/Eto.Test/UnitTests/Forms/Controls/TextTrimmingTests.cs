using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls
{
	/// <summary>
	/// Tests for <see cref="Label.Trimming"/> / <see cref="LinkButton.Trimming"/> and the wrap modes
	/// they sit beside.
	/// </summary>
	[TestFixture]
	public class TextTrimmingTests : TestBase
	{
		const string LongText = "https://www.example.com/a/rather/long/path/that/will/not/fit/in/a/narrow/control";

		// Word wrap cannot break inside a word, so comparing against a wrapped control needs text with spaces.
		const string LongSentence = "The quick brown fox jumps over the lazy dog and keeps on running for quite a while";

		const int NarrowWidth = 80;

		[Test]
		public void LabelShouldHaveDefaultTrimming() => Invoke(() =>
		{
			Assert.That(new Label().Trimming, Is.EqualTo(TextTrimming.None));
		});

		[Test]
		public void LinkButtonShouldHaveDefaults() => Invoke(() =>
		{
			var link = new LinkButton();
			Assert.Multiple(() =>
			{
				Assert.That(link.Trimming, Is.EqualTo(TextTrimming.None));
				Assert.That(link.Wrap, Is.EqualTo(WrapMode.Word));
			});
		});

		[TestCase(TextTrimming.None)]
		[TestCase(TextTrimming.CharacterEllipsis)]
		[TestCase(TextTrimming.WordEllipsis)]
		public void LabelTrimmingShouldRoundTrip(TextTrimming trimming) => Invoke(() =>
		{
			var label = new Label { Text = LongText, Wrap = WrapMode.None, Trimming = trimming };
			Assert.That(label.Trimming, Is.EqualTo(trimming));
		});

		[TestCase(TextTrimming.None)]
		[TestCase(TextTrimming.CharacterEllipsis)]
		[TestCase(TextTrimming.WordEllipsis)]
		public void LinkButtonTrimmingShouldRoundTrip(TextTrimming trimming) => Invoke(() =>
		{
			var link = new LinkButton { Text = LongText, Wrap = WrapMode.None, Trimming = trimming };
			Assert.That(link.Trimming, Is.EqualTo(trimming));
		});

		[TestCase(WrapMode.None)]
		[TestCase(WrapMode.Word)]
		[TestCase(WrapMode.Character)]
		public void LinkButtonWrapShouldRoundTrip(WrapMode wrap) => Invoke(() =>
		{
			var link = new LinkButton { Text = LongText, Wrap = wrap };
			Assert.That(link.Wrap, Is.EqualTo(wrap));
		});

		/// <summary>
		/// Trimming is a rendering mode, not a sizing one: a trimmed control still asks the layout for the
		/// room its whole text needs, and only trims once it is given less.  A control that shrank instead
		/// would silently change every layout it is already in.
		/// </summary>
		[TestCase(TextTrimming.CharacterEllipsis)]
		[TestCase(TextTrimming.WordEllipsis)]
		public void LabelTrimmingShouldNotChangePreferredSize(TextTrimming trimming) => Invoke(() =>
		{
			var untrimmed = new Label { Text = LongText, Wrap = WrapMode.None }.GetPreferredSize();
			var trimmed = new Label { Text = LongText, Wrap = WrapMode.None, Trimming = trimming }.GetPreferredSize();
			Assert.That(trimmed, Is.EqualTo(untrimmed));
		});

		[TestCase(TextTrimming.CharacterEllipsis)]
		[TestCase(TextTrimming.WordEllipsis)]
		public void LinkButtonTrimmingShouldNotChangePreferredSize(TextTrimming trimming) => Invoke(() =>
		{
			var untrimmed = new LinkButton { Text = LongText, Wrap = WrapMode.None }.GetPreferredSize();
			var trimmed = new LinkButton { Text = LongText, Wrap = WrapMode.None, Trimming = trimming }.GetPreferredSize();
			Assert.That(trimmed, Is.EqualTo(untrimmed));
		});

		/// <summary>
		/// The point of the whole thing: squeezed into a width its text does not fit, a trimmed control that
		/// is not wrapping stays one line high, where a wrapping one grows and pushes everything below it
		/// down.  That growth is what makes a long url in a narrow panel so disruptive.
		/// </summary>
		[Test]
		public void TrimmedLabelShouldNotGrowTallerThanWrapped() => Invoke(() =>
		{
			var trimmed = new Label
			{
				Text = LongSentence,
				Width = NarrowWidth,
				Wrap = WrapMode.None,
				Trimming = TextTrimming.CharacterEllipsis
			}.GetPreferredSize();
			var wrapped = new Label { Text = LongSentence, Width = NarrowWidth, Wrap = WrapMode.Word }.GetPreferredSize();

			SkipIfCannotMeasure(wrapped);
			Assert.That(trimmed.Height, Is.LessThan(wrapped.Height), "A trimmed label should stay one line where a wrapped one grows");
		});

		[Test]
		public void TrimmedLinkButtonShouldNotGrowTallerThanWrapped() => Invoke(() =>
		{
			var trimmed = new LinkButton
			{
				Text = LongSentence,
				Width = NarrowWidth,
				Wrap = WrapMode.None,
				Trimming = TextTrimming.CharacterEllipsis
			}.GetPreferredSize();
			var wrapped = new LinkButton { Text = LongSentence, Width = NarrowWidth, Wrap = WrapMode.Word }.GetPreferredSize();

			SkipIfCannotMeasure(wrapped);
			Assert.That(trimmed.Height, Is.LessThan(wrapped.Height), "A trimmed link should stay one line where a wrapped one grows");
		});

		/// <summary>
		/// WinForms measures a control that is not in a shown window as zero, so there is nothing to compare
		/// there.  That is how its backend has always behaved and has nothing to do with the trimming.
		/// </summary>
		static void SkipIfCannotMeasure(SizeF preferredSize)
		{
			if (preferredSize.Height <= 0)
				Assert.Ignore($"{Platform.Instance.GetType().Name} does not measure a control outside a shown window");
		}
	}
}
