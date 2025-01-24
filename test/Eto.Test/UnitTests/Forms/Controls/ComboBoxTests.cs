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
	}
}
