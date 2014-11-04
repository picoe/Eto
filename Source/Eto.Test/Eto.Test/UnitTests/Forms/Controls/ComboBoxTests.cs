using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture, Category("ui"), Category(TestUtils.NoTestPlatformCategory)]
	public class ComboBoxTests
	{
		[Test]
		public void InitialValuesShouldBeCorrect()
		{
			TestUtils.Invoke(() =>
			{
				var comboBox = new ComboBox();
				Assert.IsFalse(comboBox.AutoComplete, "AutoComplete should be false");
				Assert.IsFalse(comboBox.ReadOnly, "Should not be initially read only");
				Assert.IsTrue(comboBox.Enabled, "Should be enabled");
			});
		}

		[Test]
		public void TextNotMatchingItemsShouldNotHaveSelectedItem()
		{
			TestUtils.Invoke(() =>
			{
				int selectedIndexChanged = 0;
				var comboBox = new ComboBox { Items = { "Item 1", "Item 2", "Item 3" } };
				comboBox.SelectedIndexChanged += (sender, args) => selectedIndexChanged++;
				Assert.AreEqual(-1, comboBox.SelectedIndex, "Should not have an initially selected item");
				comboBox.Text = "Item Not In List";
				Assert.AreEqual(0, selectedIndexChanged, "Setting text to something not in list should not fire SelectedIndexChanged event");
				comboBox.Text = "Item 1";
				Assert.AreEqual(1, selectedIndexChanged, "Setting text to an item in the list should fire a SelectedIndexChanged event");
			});
		}
	}
}
