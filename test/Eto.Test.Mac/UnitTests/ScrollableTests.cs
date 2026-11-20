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

		[Test, ManualTest]
		public void TrackpadGestureShouldGoToControlUnderPointerWhenItHandlesMouseWheel()
		{
			ManualForm(
				"Using a trackpad:\n" +
				"1. Swipe starting over a blue box: its value changes and the content does not scroll\n" +
				"2. Swipe starting on empty space: it scrolls with momentum and bounce, and boxes passing under the pointer don't change\n" +
				"3. Flick several times in a row: scrolling continues even when a box lands under the pointer\n" +
				"4. Swipe starting over an orange box without Option held: it scrolls smoothly from the first movement\n" +
				"5. Hold Option and swipe over an orange box: its value changes and the content does not scroll",
				form =>
			{
				var layout = new DynamicLayout { ID = "layout", Padding = 20, DefaultSpacing = new Size(10, 40) };
				for (int i = 0; i < 20; i++)
				{
					var requiresOption = i % 2 == 1;
					var value = 0f;
					var label = new Label { Text = "0", TextColor = Colors.White };
					var box = new Panel
					{
						ID = $"{(requiresOption ? "orange" : "blue")}{i}", // TEMP wheel log
						Size = new Size(200, 60),
						Padding = 10,
						BackgroundColor = requiresOption ? Colors.DarkOrange : Colors.SteelBlue,
						Content = new StackLayout
						{
							ID = $"stack{i}", // TEMP wheel log
							Items = { (requiresOption ? $"orange{i}: Option + swipe" : $"blue{i}: Swipe"), label }
						}
					};
					box.MouseWheel += (sender, e) =>
					{
						System.IO.File.AppendAllText("/private/tmp/claude-501/-Users-curtis-Projects-McNeel-rhino9-src4-DotNetSDK-Eto/a5e87495-cf21-4954-9be3-1ed51de26068/scratchpad/wheel.log", $"{DateTime.Now:HH:mm:ss.fff}   {box.ID} MouseWheel delta={e.Delta} modifiers={e.Modifiers} location={e.Location}\n"); // TEMP wheel log
						if (requiresOption && !e.Modifiers.HasFlag(Keys.Alt))
							return;
						value += e.Delta.Height;
						label.Text = value.ToString("0.0");
						e.Handled = true;
					};
					layout.AddRow(box, null);
				}
				// TEMP wheel log: every scroll event before AppKit dispatches it, and the view under the pointer
				var monitor = NSEvent.AddLocalMonitorForEventsMatchingMask(NSEventMask.ScrollWheel, theEvent =>
				{
					var root = theEvent.Window?.ContentView?.Superview;
					var hit = root?.HitTest(theEvent.LocationInWindow);
					var owner = hit;
					while (owner != null && !((owner as Eto.Mac.Forms.IMacControl)?.WeakHandler?.Target is Eto.Mac.Forms.IMacViewHandler))
						owner = owner.Superview;
					var ownerId = ((owner as Eto.Mac.Forms.IMacControl)?.WeakHandler?.Target as Eto.Mac.Forms.IMacViewHandler)?.Widget?.ID;
					System.IO.File.AppendAllText("/private/tmp/claude-501/-Users-curtis-Projects-McNeel-rhino9-src4-DotNetSDK-Eto/a5e87495-cf21-4954-9be3-1ed51de26068/scratchpad/wheel.log", $"{DateTime.Now:HH:mm:ss.fff} event phase={theEvent.Phase} momentum={theEvent.MomentumPhase} dy={theEvent.ScrollingDeltaY:0.0} hit={hit?.GetType().Name} in {ownerId ?? owner?.GetType().Name}\n");
					return theEvent;
				});
				form.Closed += (sender, e) => NSEvent.RemoveMonitor(monitor);
				var scrollable = new Scrollable { Content = layout, Size = new Size(400, 400) };
				// TEMP wheel log: when the scroll view itself is tracking a gesture or its momentum
				var scrollView = (NSScrollView)scrollable.ControlObject;
				foreach (var name in new[] { "NSScrollViewWillStartLiveScrollNotification", "NSScrollViewDidEndLiveScrollNotification" })
				{
					var observer = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(name), n =>
						System.IO.File.AppendAllText("/private/tmp/claude-501/-Users-curtis-Projects-McNeel-rhino9-src4-DotNetSDK-Eto/a5e87495-cf21-4954-9be3-1ed51de26068/scratchpad/wheel.log", $"{DateTime.Now:HH:mm:ss.fff} scroll view {name}\n"), scrollView);
					form.Closed += (sender, e) => NSNotificationCenter.DefaultCenter.RemoveObserver(observer);
				}
				return scrollable;
			});
		}
	}
}