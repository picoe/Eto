using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests;

[TestFixture]
public class MouseTrackingLoopTests : TestBase
{
	/// <summary>
	/// NSTableView, NSStepper and other AppKit controls track the mouse in an event loop of their own until it is
	/// released, so the view never gets a mouseUp: of its own and handling the mouse down has to raise MouseUp
	/// from the event that ended that loop once it returns.  Without it MouseUp is never raised at all for a
	/// click on such a control.
	///
	/// ListBox is used because it has nothing of its own to fall back on, unlike Button, CheckBox and the grids,
	/// which raise MouseUp from their activation instead.
	/// </summary>
	[Test]
	public void ClickingControlThatTracksUntilMouseUpShouldRaiseMouseUp()
	{
		int mouseDownCount = 0, mouseUpCount = 0;
		Shown(form =>
		{
			var listBox = new ListBox { Items = { "one", "two", "three" } };
			listBox.MouseDown += (sender, e) => mouseDownCount++;
			listBox.MouseUp += (sender, e) => mouseUpCount++;
			form.ClientSize = new Size(200, 200);
			form.Content = new Panel { Padding = 20, Content = listBox };
			return listBox;
		},
		listBox =>
		{
			// AppKit ignores the first click while the app is coming to the front
			ClickEvents.Click(listBox);
			mouseDownCount = mouseUpCount = 0;

			// the click is over by the time this returns - the table consumes the mouse up while tracking
			ClickEvents.Click(listBox);

			Assert.Multiple(() =>
			{
				Assert.That(mouseDownCount, Is.EqualTo(1), "#1.1 MouseDown should be raised once");
				Assert.That(mouseUpCount, Is.EqualTo(1), "#1.2 MouseUp should be raised once");
			});
		});
	}
}
