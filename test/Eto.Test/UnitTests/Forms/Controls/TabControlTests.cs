using NUnit.Framework;
namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class TabControlTests : TestBase
	{
		static TabControl CreateTabControl(int hiddenIndex = -1)
		{
			var tabControl = new TabControl();
			for (int i = 0; i < 3; i++)
				tabControl.Pages.Add(new TabPage(new Label { Text = $"Page {i}" }) { Text = $"Tab {i}", Visible = i != hiddenIndex });
			return tabControl;
		}

		[Test]
		public void SelectingHiddenPageShouldNotChangeSelection()
		{
			TabControl tabControl = null;
			int changed = 0;
			Shown(form =>
			{
				tabControl = CreateTabControl(1);
				form.Content = tabControl;
			}, () =>
			{
				tabControl.SelectedIndex = 2;
				tabControl.SelectedIndexChanged += (sender, e) => changed++;

				tabControl.SelectedIndex = 1;
				Assert.That(tabControl.SelectedIndex, Is.EqualTo(2));
				Assert.That(tabControl.SelectedPage, Is.SameAs(tabControl.Pages[2]));
				Assert.That(changed, Is.EqualTo(0));
			});
		}

		[Test]
		public void HiddenFirstPageShouldNotBeSelectedInitially()
		{
			TabControl tabControl = null;
			Shown(form =>
			{
				tabControl = CreateTabControl(0);
				form.Content = tabControl;
			}, () =>
			{
				Assert.That(tabControl.SelectedIndex, Is.EqualTo(1));
			});
		}

		[Test]
		public void SelectingPageAfterHiddenPageShouldUsePageIndex()
		{
			TabControl tabControl = null;
			Shown(form =>
			{
				tabControl = CreateTabControl(0);
				form.Content = tabControl;
			}, () =>
			{
				tabControl.SelectedIndex = 2;
				Assert.That(tabControl.SelectedIndex, Is.EqualTo(2));
				Assert.That(tabControl.SelectedPage, Is.SameAs(tabControl.Pages[2]));
			});
		}

		[Test]
		public void HidingSelectedPageShouldSelectNextAndNotify()
		{
			TabControl tabControl = null;
			int changed = 0;
			Shown(form =>
			{
				tabControl = CreateTabControl();
				form.Content = tabControl;
			}, () =>
			{
				tabControl.SelectedIndex = 1;
				tabControl.SelectedIndexChanged += (sender, e) => changed++;

				tabControl.Pages[1].Visible = false;
				Assert.That(tabControl.SelectedIndex, Is.EqualTo(2));
				Assert.That(changed, Is.EqualTo(1));
			});
		}

		[Test]
		public void HidingSelectedLastPageShouldSelectPrevious()
		{
			TabControl tabControl = null;
			Shown(form =>
			{
				tabControl = CreateTabControl();
				form.Content = tabControl;
			}, () =>
			{
				tabControl.SelectedIndex = 2;
				tabControl.Pages[2].Visible = false;
				Assert.That(tabControl.SelectedIndex, Is.EqualTo(1));
			});
		}

		[Test]
		public void ShowingHiddenPageShouldKeepSelection()
		{
			TabControl tabControl = null;
			int changed = 0;
			Shown(form =>
			{
				tabControl = CreateTabControl(0);
				form.Content = tabControl;
			}, () =>
			{
				tabControl.SelectedIndex = 2;
				tabControl.SelectedIndexChanged += (sender, e) => changed++;

				tabControl.Pages[0].Visible = true;
				Assert.That(tabControl.SelectedIndex, Is.EqualTo(2));
				Assert.That(changed, Is.EqualTo(0));
			});
		}

		[Test]
		public void HiddenPageShouldBeSelectableAfterShowing()
		{
			TabControl tabControl = null;
			Shown(form =>
			{
				tabControl = CreateTabControl(1);
				form.Content = tabControl;
			}, () =>
			{
				tabControl.Pages[1].Visible = true;
				tabControl.SelectedIndex = 1;
				Assert.That(tabControl.SelectedIndex, Is.EqualTo(1));
			});
		}
	}
}
