using Eto.Test.UnitTests;
using NUnit.Framework;
using Eto.Mac.Forms.Controls;
namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
	public class ScrollableTests : TestBase
	{		
		[TestCase(300, 100, false, true, true)]
		[TestCase(100, 300, true, false, true)]
		[TestCase(300, 300, false, false, true)]
		[TestCase(100, 100, true, true, true)]
		
		[TestCase(300, 100, false, true, false)]
		[TestCase(100, 300, true, false, false)]
		[TestCase(300, 300, false, false, false)]
		[TestCase(100, 100, true, true, false)]
		
		[TestCase(200, 200, false, false, false)]
		[TestCase(200, 100, false, true, false)]
		[TestCase(100, 200, true, false, false)]
		public void ScrollableShouldNotHaveScrollBarsWhenContentFits(int width, int height, bool hbar, bool vbar, bool legacy)
		{
			ShownAsync(form =>
			{
				var scrollable = new Scrollable
				{
					Content = new Panel { Size = new Size(200, 200) },
					Size = new Size(width, height),
					Border = BorderType.None
				};
				var handler = scrollable.Handler as ScrollableHandler;
				if (legacy)
					handler.Control.ScrollerStyle = NSScrollerStyle.Legacy;
				else
					handler.Control.ScrollerStyle = NSScrollerStyle.Overlay;
				return scrollable;
			}, async scrollable =>
			{
				await Task.Delay(100); // give time for layout to occur
				var handler = scrollable.Handler as ScrollableHandler;

				if (hbar)
					Assert.That(handler.Control.HorizontalScroller.IsHiddenOrHasHiddenAncestor, Is.False, "Scrollable should have horizontal scrollbar when content does not fit");
				else
					Assert.That(handler.Control.HorizontalScroller.IsHiddenOrHasHiddenAncestor, Is.True, "Scrollable should not have horizontal scrollbar when content fits");
					
				if (vbar)
					Assert.That(handler.Control.VerticalScroller.IsHiddenOrHasHiddenAncestor, Is.False, "Scrollable should have vertical scrollbar when content does not fit");
				else
					Assert.That(handler.Control.VerticalScroller.IsHiddenOrHasHiddenAncestor, Is.True, "Scrollable should not have vertical scrollbar when content fits");
			});

		}
		
		/// <summary>
		/// When we set a BackgroundColor of a panel, we set WantsLayer and CanDrawSubviewsIntoLayer to true.
		/// This causes problems with a Scrollable as it makes the content duplicate when scrolling as it was drawn
		/// to the parent layer.
		/// </summary>
		[Test, ManualTest]
		public void ScrollableInLayerShouldNotDuplicateContents()
		{
			ManualForm("Scroll the box, the content should not duplicate", form =>
			{
				form.BackgroundColor = Colors.White;
				var panel = new Panel { Size = new Size(200, 200) };
				var button = new Button { Text = "Click Me" };
				var content = new TableLayout
				{
					Rows = {
						button,
						panel
					}
				};

				button.Click += (sender, e) => 
				{
					var scrollContent = new TableLayout();
					for (int i = 0; i < 50; i++)
					{
						scrollContent.Rows.Add(new CheckBox { Text = "Check Box " + i });
					}

					panel.Content = new TableLayout
					{
						Padding = 20,
						Rows = { new Scrollable { Content = scrollContent } }
					};
				};

				return content;
			});
		}
	}
}