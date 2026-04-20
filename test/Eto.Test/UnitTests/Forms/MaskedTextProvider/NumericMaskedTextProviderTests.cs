using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.MaskedTextProvider
{
	[TestFixture]
	public class NumericMaskedTextProviderTests
	{
		[Test]
		public void TextShouldShowAtLeastDecimalPlaces()
		{
			var provider = new NumericMaskedTextProvider<double>
			{
				DecimalPlaces = 2,
				MaximumDecimalPlaces = 4,
				Value = 123
			};
			provider.CommitText();

			Assert.That(provider.Text, Is.EqualTo("123.00"));
			Assert.That(provider.DisplayText, Is.EqualTo("123.00"));
		}

		[Test]
		public void TextShouldShowUpToMaximumDecimalPlaces()
		{
			var provider = new NumericMaskedTextProvider<double>
			{
				DecimalPlaces = 2,
				MaximumDecimalPlaces = 4,
				Value = 123.456789
			};
			provider.CommitText();

			Assert.That(provider.Text, Is.EqualTo("123.4568"));
			Assert.That(provider.DisplayText, Is.EqualTo("123.4568"));
		}

		[Test]
		public void TextShouldRetainSignificantDecimalsUpToMaximum()
		{
			var provider = new NumericMaskedTextProvider<double>
			{
				DecimalPlaces = 2,
				MaximumDecimalPlaces = 4,
				Value = 123.4
			};
			provider.CommitText();

			Assert.That(provider.Text, Is.EqualTo("123.40"));

			provider.Value = 123.456;
			provider.CommitText();

			Assert.That(provider.Text, Is.EqualTo("123.456"));
		}

		[Test]
		public void DecimalPlacePropertiesShouldStayInRange()
		{
			var provider = new NumericMaskedTextProvider<double>();

			provider.DecimalPlaces = 3;
			Assert.That(provider.DecimalPlaces, Is.EqualTo(3));
			Assert.That(provider.MaximumDecimalPlaces, Is.GreaterThanOrEqualTo(3));

			provider.MaximumDecimalPlaces = 1;
			Assert.That(provider.DecimalPlaces, Is.EqualTo(1));
			Assert.That(provider.MaximumDecimalPlaces, Is.EqualTo(1));
		}

		[Test]
		public void IncompleteInputShouldRemainEditable()
		{
			var provider = new NumericMaskedTextProvider<double>
			{
				DecimalPlaces = 2,
				MaximumDecimalPlaces = 4
			};

			provider.Text = "-";
			Assert.That(provider.Text, Is.EqualTo("-"));

			provider.Text = "12.";
			Assert.That(provider.Text, Is.EqualTo("12."));
		}

		[Test]
		public void InsertShouldUseDisplayedDecimalDigitsForCaretPositions()
		{
			var provider = new NumericMaskedTextProvider<double>
			{
				DecimalPlaces = 3,
				MaximumDecimalPlaces = 4,
				Value = 1
			};

			var position = provider.Text.Length;
			var inserted = provider.Insert('2', ref position);
			provider.CommitText();

			Assert.That(inserted, Is.True);
			Assert.That(provider.Text, Is.EqualTo("1.0002"));
			Assert.That(position, Is.EqualTo(provider.Text.Length));
		}

		[Test]
		public void DeleteShouldPreserveEditableDecimalsUntilFormatted()
		{
			var provider = new NumericMaskedTextProvider<double>
			{
				DecimalPlaces = 4,
				MaximumDecimalPlaces = 5,
				Value = 1.00002
			};
			provider.CommitText();

			var position = provider.Text.Length;
			var deleted = provider.Delete(ref position, 1, false);

			Assert.That(deleted, Is.True);
			Assert.That(provider.Text, Is.EqualTo("1.0000"));

			provider.CommitText();

			Assert.That(provider.Text, Is.EqualTo("1.0000"));
		}
	}
}
