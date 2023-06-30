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
				Assert.LessOrEqual(control.Width, 0, "#1.1");
				Assert.LessOrEqual(control.Height, 0, "#1.2");
				Assert.AreEqual(new Point(0, 0), control.Location, "#1.3");

				holder.Content = control;

				// layout is suspended or deferred so nothing is set up yet
				// Gtk is annoying and returns 1,1 for size at this stage, others return 0,0.
				Assert.LessOrEqual(control.Width, 1, "#2.1");
				Assert.LessOrEqual(control.Height, 1, "#2.2");
				
				// macOS gives us a "flipped" view of the location at this point..
				// the value of Location isn't valid here anyway.
				if (!Platform.Instance.IsMac)
					Assert.AreEqual(new Point(0, 0), control.Location, "#2.3");

				holder.ResumeLayout();

				if (!Platform.Instance.IsWinForms)
				{
					// Gtk, Wpf, and Mac all use deferred layouts so it still isn't set up here.
					Assert.LessOrEqual(control.Width, 1, "#3.1");
					Assert.LessOrEqual(control.Height, 1, "#3.2");
					
					if (!Platform.Instance.IsMac)
						Assert.AreEqual(new Point(0, 0), control.Location, "#3.3");
				}
				else
				{
					// At this point WinForms is all set up.
					Assert.AreEqual(100, control.Width, "#3.1");
					Assert.AreEqual(100, control.Height, "#3.2");
					Assert.AreEqual(new Point(0, 0), control.Location, "#3.3");
					Assert.AreEqual(new Point(100, 100), Point.Round(table.PointFromScreen(control.PointToScreen(PointF.Empty))), "#3.4");
				}

				// force a layout
				holder.UpdateLayout();

				// everything should now be set up as we expect.
				Assert.AreEqual(100, control.Width, "#4.1");
				Assert.AreEqual(100, control.Height, "#4.2");
				Assert.AreEqual(new Point(0, 0), control.Location, "#4.3");
				Assert.AreEqual(new Point(100, 100), Point.Round(table.PointFromScreen(control.PointToScreen(PointF.Empty))), "#4.4");
			});
		}
	}
}
