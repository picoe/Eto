using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Layout
{
	[TestFixture]
	public class LayoutTests : TestBase
	{
		[Test]
		public void UpdateLayoutShouldSetAllSizes()
		{
			Panel holder = null;
			TableLayout table = null;
			Shown(form =>
			{
				holder = new Panel { Size = new Size(100, 100) };
				table = new TableLayout
				{
					Rows =
					{
						new TableRow(new Panel { Size = new Size(100, 100)}),
						new TableRow(new TableCell(), holder)
					}
				};
				form.Content = table;
			},
			() =>
			{
				holder.SuspendLayout();

				var control = new Panel();
				control.Content = "Hello then!";
				Assert.That(control.Width, Is.LessThanOrEqualTo(0), "#1.1");
				Assert.That(control.Height, Is.LessThanOrEqualTo(0), "#1.2");
				Assert.That(control.Location, Is.EqualTo(new Point(0, 0)), "#1.3");

				holder.Content = control;

				// layout is suspended or deferred so nothing is set up yet
				// Gtk is annoying and returns 1,1 for size at this stage, others return 0,0.
				Assert.That(control.Width, Is.LessThanOrEqualTo(1), "#2.1");
				Assert.That(control.Height, Is.LessThanOrEqualTo(1), "#2.2");

				// macOS gives us a "flipped" view of the location at this point..
				// the value of Location isn't valid here anyway.
				if (!Platform.Instance.IsMac)
					Assert.That(control.Location, Is.EqualTo(new Point(0, 0)), "#2.3");

				holder.ResumeLayout();

				if (!Platform.Instance.IsWinForms)
				{
					// Gtk, Wpf, and Mac all use deferred layouts so it still isn't set up here.
					Assert.That(control.Width, Is.LessThanOrEqualTo(1), "#3.1");
					Assert.That(control.Height, Is.LessThanOrEqualTo(1), "#3.2");

					if (!Platform.Instance.IsMac)
						Assert.That(control.Location, Is.EqualTo(new Point(0, 0)), "#3.3");
				}
				else
				{
					// At this point WinForms is all set up.
					Assert.That(control.Width, Is.EqualTo(100), "#3.1");
					Assert.That(control.Height, Is.EqualTo(100), "#3.2");
					Assert.That(control.Location, Is.EqualTo(new Point(0, 0)), "#3.3");
					Assert.That(Point.Round(table.PointFromScreen(control.PointToScreen(PointF.Empty))), Is.EqualTo(new Point(100, 100)), "#3.4");
				}

				// force a layout
				holder.UpdateLayout();

				// everything should now be set up as we expect.
				Assert.That(control.Width, Is.EqualTo(100), "#4.1");
				Assert.That(control.Height, Is.EqualTo(100), "#4.2");
				Assert.That(control.Location, Is.EqualTo(new Point(0, 0)), "#4.3");
				Assert.That(Point.Round(table.PointFromScreen(control.PointToScreen(PointF.Empty))), Is.EqualTo(new Point(100, 100)), "#4.4");
			});
		}
		
		[Test]
		public void LabelShouldGetProperSizeWhenContainerSizeIsSetInScrollable()
		{
			Label label = null;
			ShownAsync(form =>
			{
				label = new Label { Text = "This label should be fully visible, but then mucks up the scroll size way too much", Wrap = WrapMode.Word };
				var container = new Panel { Content = label, Width = 10 };
				container.BackgroundColor = Colors.Blue;
				var scrollable = new Scrollable
				{
					Size = new Size(300, 300),
					Content = container
				};
				return scrollable;
			},
			async scrollable =>
			{
				await Task.Delay(100); // wait to see the results
				Assert.That(label.Width, Is.GreaterThan(20), "#1");
				Assert.That(scrollable.Content.Size, Is.EqualTo(scrollable.ClientSize), "#2");
			});	
		}		
		[Test]
		public void LabelShouldGetProperSizeWhenContainerSizeIsSet()
		{
			Label label = null;
			ShownAsync(form =>
			{
				label = new Label { Text = "This label should be fully visible, but then mucks up the scroll size way too much", Wrap = WrapMode.Word };
				var container = new Panel { Content = label, Width = 10 };
				container.BackgroundColor = Colors.Blue;
				var panel = new Panel
				{
					Size = new Size(300, 300),
					Content = container
				};
				return panel;
			},
			async panel =>
			{
				await Task.Delay(100); // wait to see the results
				Assert.That(label.Width, Is.GreaterThan(20), "#1");
				Assert.That(panel.Content.Size, Is.EqualTo(panel.ClientSize), "#2");
			});	
		}		
	}
}
