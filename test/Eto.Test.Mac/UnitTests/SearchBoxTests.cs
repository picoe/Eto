using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
	public class SearchBoxTests : TestBase
	{
		static double SearchButtonMiddle(SearchBox box)
		{
			var field = (NSSearchField)box.ControlObject;
			return ((NSSearchFieldCell)field.Cell).SearchButtonRectForBounds(field.Bounds).GetMidX();
		}

		/// <summary>
		/// The modern search field lays out its icon in an inner view that doesn't inherit the field's direction.
		/// </summary>
		[Test]
		public void SearchIconShouldBeAtLeadingEdgeInRightToLeft()
		{
			SearchBox shown = null, changed = null;
			ShownAsync(form =>
			{
				shown = new SearchBox { Width = 200, LayoutDirection = LayoutDirection.RightToLeft };
				changed = new SearchBox { Width = 200, LayoutDirection = LayoutDirection.LeftToRight };
				form.Content = new TableLayout(shown, changed);
			}, async () =>
			{
				Assert.That(SearchButtonMiddle(shown), Is.GreaterThan(shown.Width / 2), "Shown in right-to-left");
				Assert.That(SearchButtonMiddle(changed), Is.LessThan(changed.Width / 2), "Left-to-right");

				changed.LayoutDirection = LayoutDirection.RightToLeft;
				await Task.Delay(200);
				Assert.That(SearchButtonMiddle(changed), Is.GreaterThan(changed.Width / 2), "Changed to right-to-left");

				changed.LayoutDirection = LayoutDirection.LeftToRight;
				await Task.Delay(200);
				Assert.That(SearchButtonMiddle(changed), Is.LessThan(changed.Width / 2), "Changed back to left-to-right");
			});
		}
	}
}
