using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Gtk.UnitTests
{
	[TestFixture]
	public class ScrollableTests : TestBase
	{
		[Test]
		public void AutoSizedWindowShouldSizeToDisplayedContentHeight()
		{
			// An auto-sized window sizes its height to the height of the scrollable content measured for
			// the width it is actually displayed at. GtkScrolledWindow ignores the width in its
			// height-for-width request, so unless ScrollableHandler.EtoScrolledWindow measures the content
			// itself the window is sized to the much taller height that the wrapping label has at the
			// narrow preferred width of its container.
			// Gtk-specific as the shared ScrollableTests can only check this through ScrollSize, which
			// reports the content extent regardless of how tall the window ends up.
			Form theForm = null;
			Label label = null;
			Shown(form =>
			{
				theForm = form;
				form.AutoSize = true;
				form.Width = 500;
				label = new Label
				{
					Text = "This is a long label that wraps to many lines when narrow but fits in fewer lines when wider",
					Wrap = WrapMode.Word
				};
				var content = new StackLayout
				{
					HorizontalContentAlignment = HorizontalAlignment.Stretch,
					Width = 100,
					Items = { label }
				};
				return new Scrollable
				{
					Content = content,
					ExpandContentWidth = true,
					ExpandContentHeight = false
				};
			},
			scrollable =>
			{
				var narrowHeight = label.GetPreferredSize(new SizeF(100, float.PositiveInfinity)).Height;
				var displayedHeight = scrollable.Content.Size.Height;
				Assert.Multiple(() =>
				{
					Assert.That(displayedHeight, Is.LessThan(narrowHeight), "#1 the label should be shorter when displayed at the expanded width");
					Assert.That(theForm.ClientSize.Height, Is.LessThan(narrowHeight), "#2 Window should not size to the height the content has at its narrow preferred width");
					Assert.That(theForm.ClientSize.Height, Is.GreaterThanOrEqualTo(displayedHeight), "#3 Window should be tall enough to show the content");
				});
			});
		}
	}
}
