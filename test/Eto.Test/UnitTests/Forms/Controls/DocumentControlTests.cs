using NUnit.Framework;
namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class DocumentControlTests : TestBase
	{
		[Test]
		public void LogicalParentShouldChangeWhenAddedOrRemoved()
		{
			Invoke(() =>
			{
				var ctl = new DocumentControl();
				var child = new Panel { Size = new Size(100, 100) };
				var page = new DocumentPage(child);
				Assert.That(child.Parent, Is.EqualTo(page), "#1");
				ctl.Pages.Add(page);
				Assert.That(ctl, Is.EqualTo(page.Parent), "#2");
				ctl.Pages.RemoveAt(0);
				Assert.That(page.Parent, Is.Null, "#3");
				page.Content = null;
				Assert.That(child.Parent, Is.Null, "#4");
			});
		}

		[Test]
		public void LoadedEventsShouldPropegate()
		{
			Panel child1 = null;
			Panel child2 = null;
			DocumentPage page1 = null;
			DocumentPage page2 = null;

			Shown(form =>
			{
				var ctl = new DocumentControl();

				child1 = new Panel { Size = new Size(100, 100) };
				ctl.Pages.Add(page1 = new DocumentPage(child1) { Text = "Page 1" });

				Assert.That(child1.Loaded, Is.False, "#1");

				child2 = new Panel { Size = new Size(100, 100) };
				ctl.Pages.Add(page2 = new DocumentPage(child2));

				Assert.That(child2.Loaded, Is.False, "#2");
				return ctl;
			}, ctl =>
			{
				Assert.That(child1.Loaded, Is.True, "#3");
				page1.Content = new Panel();
				Assert.That(child1.Loaded, Is.False, "#4");

				ctl.SelectedIndex = 1;

				Assert.That(child2.Loaded, Is.True, "#5");
				ctl.Pages.RemoveAt(1);
				Assert.That(child2.Loaded, Is.False, "#6");
				Assert.That(page2.Loaded, Is.False, "#7");
			});
		}
	}
}
