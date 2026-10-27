using NUnit.Framework;
using Range = Eto.Forms.Range;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class TextChangingEventArgsTests
	{
		public static IEnumerable<object[]> GetTextChangingCases()
		{
			yield return new object[] { "", "some new text", "some new text", 0, 0 };
			yield return new object[] { null, "some new text", "some new text", 0, 0 };
			yield return new object[] { "some old text", "", "", 0, 13 };
			yield return new object[] { "some old text", null, "", 0, 13 };
			yield return new object[] { "some old text", "some new text", "new", 5, 3 };
			yield return new object[] { "some old", "some new text", "new text", 5, 3 };
			yield return new object[] { "some old text", "some new", "new", 5, 8 };
			yield return new object[] { "some old text", "new text", "new", 0, 8 };
			yield return new object[] { "some old and boring text", "some new text", "new", 5, 14 };
		}

		[TestCaseSource(nameof(GetTextChangingCases))]
		public void OldAndNewTextShouldCalculateRangeAndText(string oldText, string newText, string text, int rangeStart, int rangeLength)
		{
			var args = new TextChangingEventArgs(oldText, newText, false);

			Assert.That(args.OldText, Is.EqualTo(oldText ?? string.Empty), "#1");
			Assert.That(args.NewText, Is.EqualTo(newText ?? string.Empty), "#2");
			Assert.That(args.Range, Is.EqualTo(Range.FromLength(rangeStart, rangeLength)), "#3");
			Assert.That(args.Text, Is.EqualTo(text), "#4");
		}

		[TestCaseSource(nameof(GetTextChangingCases))]
		public void OldAndRangeShouldCalculateNewText(string oldText, string newText, string text, int rangeStart, int rangeLength)
		{
			var args = new TextChangingEventArgs(text, Range.FromLength(rangeStart, rangeLength), oldText, false);

			Assert.That(args.OldText, Is.EqualTo(oldText ?? string.Empty), "#1");
			Assert.That(args.NewText, Is.EqualTo(newText ?? string.Empty), "#2");
			Assert.That(args.Range, Is.EqualTo(Range.FromLength(rangeStart, rangeLength)), "#3");
			Assert.That(args.Text, Is.EqualTo(text), "#4");
		}

		[TestCase(TextChangeSource.Programmatic, false)]
		[TestCase(TextChangeSource.Unknown, true)]
		[TestCase(TextChangeSource.Keyboard, true)]
		[TestCase(TextChangeSource.Composition, true)]
		[TestCase(TextChangeSource.Paste, true)]
		[TestCase(TextChangeSource.Cut, true)]
		public void SourceShouldRoundTripAndDeriveFromUser(TextChangeSource source, bool expectedFromUser)
		{
			var args = new TextChangingEventArgs("old", "new", source);
			Assert.That(args.Source, Is.EqualTo(source), "#1");
			Assert.That(args.FromUser, Is.EqualTo(expectedFromUser), "#2");
		}

		[TestCase(true, TextChangeSource.Unknown)]
		[TestCase(false, TextChangeSource.Programmatic)]
		public void LegacyFromUserShouldMapToSource(bool fromUser, TextChangeSource expectedSource)
		{
			// The legacy bool constructors must keep FromUser working and map to a sensible source.
			var byOldNew = new TextChangingEventArgs("old", "new", fromUser);
			Assert.That(byOldNew.Source, Is.EqualTo(expectedSource), "#1");
			Assert.That(byOldNew.FromUser, Is.EqualTo(fromUser), "#2");

			var byRange = new TextChangingEventArgs("text", Range.FromLength(0, 0), fromUser);
			Assert.That(byRange.Source, Is.EqualTo(expectedSource), "#3");
			Assert.That(byRange.FromUser, Is.EqualTo(fromUser), "#4");

			var byRangeOld = new TextChangingEventArgs("text", Range.FromLength(0, 0), "old", fromUser);
			Assert.That(byRangeOld.Source, Is.EqualTo(expectedSource), "#5");
			Assert.That(byRangeOld.FromUser, Is.EqualTo(fromUser), "#6");
		}
	}
}
