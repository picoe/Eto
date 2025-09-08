using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class ComboBoxTests : ListControlTests<ComboBox>
	{
		[Test]
		public void InitialValuesShouldBeCorrect()
		{
			Invoke(() =>
			{
				var comboBox = new ComboBox();
				Assert.That(comboBox.AutoComplete, Is.False, "AutoComplete should be false");
				Assert.That(comboBox.ReadOnly, Is.False, "Should not be initially read only");
				Assert.That(comboBox.Enabled, Is.True, "Should be enabled");
			});
		}

		[Test]
		public void TextNotMatchingItemsShouldNotHaveSelectedItem()
		{
			Invoke(() =>
			{
				int selectedIndexChanged = 0;
				var comboBox = new ComboBox { Items = { "Item 1", "Item 2", "Item 3" } };
				comboBox.SelectedIndexChanged += (sender, args) => selectedIndexChanged++;
				Assert.That(comboBox.SelectedIndex, Is.EqualTo(-1), "Should not have an initially selected item");
				comboBox.Text = "Item Not In List";
				Assert.That(selectedIndexChanged, Is.EqualTo(0), "Setting text to something not in list should not fire SelectedIndexChanged event");
				comboBox.Text = "Item 1";
				Assert.That(selectedIndexChanged, Is.EqualTo(1), "Setting text to an item in the list should fire a SelectedIndexChanged event");
			});
		}

		[Test]
		public void SettingTextToItemShouldSetSelectedItem()
		{
			Invoke(() =>
			{
				var comboBox = new ComboBox { Items = { "Item 1", "Item 2", "Item 3" } };
				comboBox.Text = "Item 2";
				Assert.That(comboBox.SelectedIndex, Is.EqualTo(1), "SelectedIndex should be 1 when setting text to 'Item 2'");
				Assert.That(comboBox.Text, Is.EqualTo("Item 2"), "Text should be 'Item 2'");
			});
		}

		[Test]
		public void SettingTextToValueNotInItemsShouldNotSetSelectedItem()
		{
			Invoke(() =>
			{
				var comboBox = new ComboBox { Items = { "Item 1", "Item 2", "Item 3" } };
				comboBox.SelectedIndex = 0;
				Assert.That(comboBox.SelectedIndex, Is.EqualTo(0), "SelectedIndex should be 0 when setting to first item");
				Assert.That(comboBox.Text, Is.EqualTo("Item 1"), "Text should be 'Item 1'");
				
				comboBox.Text = "Item Not In List";
				Assert.That(comboBox.SelectedIndex, Is.EqualTo(-1), "SelectedIndex should be -1 when setting text to something not in the list");
				Assert.That(comboBox.Text, Is.EqualTo("Item Not In List"), "Text should be 'Item Not In List'");
			});
		}
	}
}
