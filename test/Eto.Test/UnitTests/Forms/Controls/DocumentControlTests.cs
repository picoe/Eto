using System;
using NUnit.Framework;
using Eto.Forms;
using Eto.Drawing;

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
				Assert.AreEqual(page, child.Parent, "#1");
				ctl.Pages.Add(page);
				Assert.AreEqual(page.Parent, ctl, "#2");
				ctl.Pages.RemoveAt(0);
				Assert.IsNull(page.Parent, "#3");
				page.Content = null;
				Assert.IsNull(child.Parent, "#4");
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

				Assert.IsFalse(child1.Loaded, "#1");

				child2 = new Panel { Size = new Size(100, 100) };
				ctl.Pages.Add(page2 = new DocumentPage(child2));

				Assert.IsFalse(child2.Loaded, "#2");
				return ctl;
			}, ctl =>
			{
				Assert.IsTrue(child1.Loaded, "#3");
				page1.Content = new Panel();
				Assert.IsFalse(child1.Loaded, "#4");

				ctl.SelectedIndex = 1;

				Assert.IsTrue(child2.Loaded, "#5");
				ctl.Pages.RemoveAt(1);
				Assert.IsFalse(child2.Loaded, "#6");
				Assert.IsFalse(page2.Loaded, "#7");
			});
		}
	}
}
