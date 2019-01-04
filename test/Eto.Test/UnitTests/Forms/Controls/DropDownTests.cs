using System;
using NUnit.Framework;
using Eto.Forms;
namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class DropDownTests : ListControlTests<DropDown>
	{
		static void TestDropDownSelection(DropDown dropDown, object item1, object item2, object item3, bool useIndex = false)
		{
			int selectedIndexChanged = 0;
			int selectedValueChanged = 0;
			int selectedKeyChanged = 0;

			dropDown.SelectedIndexChanged += (sender, e) => selectedIndexChanged++;
			dropDown.SelectedValueChanged += (sender, e) => selectedValueChanged++;
			dropDown.SelectedKeyChanged += (sender, e) => selectedKeyChanged++;

			// set up data
			dropDown.DataStore = new[] { item1, item2, item3 };
			if (useIndex)
				dropDown.SelectedIndex = 1;
			else
				dropDown.SelectedValue = item2;

			Assert.AreEqual(1, dropDown.SelectedIndex);
			Assert.AreEqual(item2, dropDown.SelectedValue);
			Assert.AreEqual(1, selectedIndexChanged);
			Assert.AreEqual(1, selectedValueChanged);
			Assert.AreEqual(1, selectedKeyChanged);

			// change the list
			dropDown.DataStore = new[] { item1, item3, item2 };
			Assert.AreEqual(2, dropDown.SelectedIndex);
			Assert.AreEqual(item2, dropDown.SelectedValue);
			Assert.AreEqual(2, selectedIndexChanged);
			Assert.AreEqual(1, selectedValueChanged);
			Assert.AreEqual(1, selectedKeyChanged);

			// change again, but selected value does not exist
			dropDown.DataStore = new[] { item1, item3 };
			Assert.AreEqual(-1, dropDown.SelectedIndex);
			Assert.IsNull(dropDown.SelectedValue);
			Assert.AreEqual(3, selectedIndexChanged);
			Assert.AreEqual(2, selectedValueChanged);
			Assert.AreEqual(2, selectedKeyChanged);

			// add it back, still should not be selected now that it was deselected!
			dropDown.DataStore = new[] { item2, item1, item3 };
			Assert.AreEqual(-1, dropDown.SelectedIndex);
			Assert.IsNull(dropDown.SelectedValue);
			Assert.AreEqual(3, selectedIndexChanged);
			Assert.AreEqual(2, selectedValueChanged);
			Assert.AreEqual(2, selectedKeyChanged);
		}

		[Test]
		public void DropDownShouldKeepSelectedValueWhenSettingDataStoreWithStrings()
		{
			Invoke(() =>
			{
				var item1 = "Item 1";
				var item2 = "Item 2";
				var item3 = "Item 3";

				var dropDown = new DropDown();

				TestDropDownSelection(dropDown, item1, item2, item3);
			});
		}

		class CustomDropDownItem
		{
			public string Text { get; set; }
		}

		[Test]
		public void DropDownShouldKeepSelectedValueWhenSettingDataStoreWithObjects()
		{
			Invoke(() =>
			{
				// text is same to make sure matching is based on equality, not value of text
				var item1 = new CustomDropDownItem { Text = "Item" };
				var item2 = new CustomDropDownItem { Text = "Item" };
				var item3 = new CustomDropDownItem { Text = "Item" };

				var dropDown = new DropDown();
				dropDown.ItemTextBinding = Eto.Forms.Binding.Property((CustomDropDownItem i) => i.Text);

				TestDropDownSelection(dropDown, item1, item2, item3);
			});
		}

		[Test]
		public void DropDownShouldKeepSelectedValueWhenSettingDataStoreWithSelectedIndex()
		{
			Invoke(() =>
			{
				var item1 = "Item 1";
				var item2 = "Item 2";
				var item3 = "Item 3";

				var dropDown = new DropDown();

				TestDropDownSelection(dropDown, item1, item2, item3, true);
			});
		}

		[Test]
		public void SettingDataStoreToNullWithSelectedItemShouldNotCrash()
		{
			Invoke(() =>
			{
				var dropDown = new DropDown();

				var items = new[] { "Item 1", "Item 2", "Item 3" };

				dropDown.DataStore = items;
				dropDown.SelectedIndex = 1;
				// sanity check
				Assert.AreEqual(items[1], dropDown.SelectedValue, "#1");

				dropDown.DataStore = null;
				Assert.AreEqual(-1, dropDown.SelectedIndex, "#2.1");
				Assert.IsNull(dropDown.SelectedValue, "#2.2");
			});
		}
	}
}
