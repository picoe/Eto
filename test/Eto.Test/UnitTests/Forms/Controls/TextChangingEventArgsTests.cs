using System;
using System.Collections.Generic;
using Eto.Forms;
using NUnit.Framework;

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

			Assert.AreEqual(oldText ?? string.Empty, args.OldText, "#1");
			Assert.AreEqual(newText ?? string.Empty, args.NewText, "#2");
			Assert.AreEqual(Range.FromLength(rangeStart, rangeLength), args.Range, "#3");
			Assert.AreEqual(text, args.Text, "#4");
		}

		[TestCaseSource(nameof(GetTextChangingCases))]
		public void OldAndRangeShouldCalculateNewText(string oldText, string newText, string text, int rangeStart, int rangeLength)
		{
			var args = new TextChangingEventArgs(text, Range.FromLength(rangeStart, rangeLength), oldText, false);

			Assert.AreEqual(oldText ?? string.Empty, args.OldText, "#1");
			Assert.AreEqual(newText ?? string.Empty, args.NewText, "#2");
			Assert.AreEqual(Range.FromLength(rangeStart, rangeLength), args.Range, "#3");
			Assert.AreEqual(text, args.Text, "#4");
		}
	}
}
