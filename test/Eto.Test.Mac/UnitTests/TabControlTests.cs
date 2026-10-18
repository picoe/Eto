using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
	public class TabControlTests : TestBase
	{
		static string[] NativeTabs(TabControl tabControl) => ((NSTabView)tabControl.ControlObject).Items.Select(r => r.Label).ToArray();

		[Test]
		public void HiddenPageShouldNotShowTab()
		{
			TabControl tabControl = null;
			Shown(form =>
			{
				tabControl = new TabControl();
				tabControl.Pages.Add(new TabPage { Text = "A" });
				tabControl.Pages.Add(new TabPage { Text = "B", Visible = false });
				tabControl.Pages.Add(new TabPage { Text = "C" });
				form.Content = tabControl;
			}, () =>
			{
				Assert.That(NativeTabs(tabControl), Is.EqualTo(new[] { "A", "C" }));

				tabControl.SelectedIndex = 2;
				Assert.That(tabControl.SelectedIndex, Is.EqualTo(2));

				tabControl.Pages[1].Visible = true;
				Assert.That(NativeTabs(tabControl), Is.EqualTo(new[] { "A", "B", "C" }));
				Assert.That(tabControl.SelectedIndex, Is.EqualTo(2));

				tabControl.Pages[0].Visible = false;
				Assert.That(NativeTabs(tabControl), Is.EqualTo(new[] { "B", "C" }));
				Assert.That(tabControl.SelectedIndex, Is.EqualTo(2));
			});
		}
	}
}
