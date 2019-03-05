using System;
using NUnit.Framework;
using Eto.Forms;
namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class SegmentedButtonTests : TestBase
	{
		[Test, ManualTest]
		public void ClickingSelectedSegmentShouldNotTriggerSelectionChanged()
		{
			var selectedIndexesChangedCount = 0;
			var itemClickCount = 0;
			var clickCount = 0;
			SegmentedItem itemClicked = null;
			SegmentedItem itemItemClicked = null;
			SegmentedItem itemExpected = null;

			Form(form =>
			{
				itemExpected = new ButtonSegmentedItem { Text = "Click Me!", Selected = true };
				var segmentedButton = new SegmentedButton
				{
					SelectionMode = SegmentedSelectionMode.Single,
					Items = { "First", itemExpected, "Last" }
				};


				segmentedButton.SelectedIndexesChanged += (sender, e) => selectedIndexesChangedCount++;
				segmentedButton.ItemClick += (sender, e) =>
				{
					itemItemClicked = e.Item;
					itemClickCount++;
				};
				itemExpected.Click += (sender, e) =>
				{
					itemClicked = sender as SegmentedItem;
					clickCount++;
					Application.Instance.AsyncInvoke(form.Close);
				};

				Assert.IsTrue(itemExpected.Selected, "#1.1");

				form.Content = new StackLayout
				{
					Spacing = 10,
					Padding = 10,
					Items =
					{ 
						"Click the selected segment",
						segmentedButton
					}
				};
			}, -1);

			Assert.AreEqual(0, selectedIndexesChangedCount, "#2.1");
			Assert.AreEqual(1, itemClickCount, "#2.2");
			Assert.AreEqual(1, clickCount, "#2.3");

			Assert.IsNotNull(itemExpected, "#3.1");
			Assert.AreSame(itemExpected, itemClicked, "#3.2");
			Assert.AreSame(itemExpected, itemItemClicked, "#3.3");
		}

		[Test, InvokeOnUI]
		public void SettingSelectedOnItemShouldChangeSelection()
		{
			var segmentedButton = new SegmentedButton
			{
				Items = { "Item1", "Item2", "Item3" }
			};

			var selectedIndexesChangedCount = 0;
			segmentedButton.SelectedIndexesChanged += (sender, e) => selectedIndexesChangedCount++;

			segmentedButton.Items[0].Selected = true;
		}

		[Test, InvokeOnUI]
		public void SettingMultipleSelectedInSingleModeShouldOnlyHaveOneSelectedItem()
		{
			var item1 = new ButtonSegmentedItem { Text = "Item1", Selected = true };
			var item2 = new ButtonSegmentedItem { Text = "Item2", Selected = true };
			var item3 = new ButtonSegmentedItem { Text = "Item3", Selected = true };

			var segmentedButton = new SegmentedButton
			{
				SelectionMode = SegmentedSelectionMode.Single
			};

			var selectedIndexesChangedCount = 0;
			segmentedButton.SelectedIndexesChanged += (sender, e) => selectedIndexesChangedCount++;

			segmentedButton.Items.Add(item1);
			Assert.AreEqual(1, selectedIndexesChangedCount, "#1.1");
			segmentedButton.Items.Add(item2);
			Assert.AreEqual(2, selectedIndexesChangedCount, "#1.2");
			segmentedButton.Items.Add(item3);
			Assert.AreEqual(3, selectedIndexesChangedCount, "#1.3");

			Assert.AreEqual(2, segmentedButton.SelectedIndex, "#2.1");
			Assert.AreSame(item3, segmentedButton.SelectedItem, "#2.2");
			CollectionAssert.AreEqual(new[] { item3 }, segmentedButton.SelectedItems, "#2.3");
			CollectionAssert.AreEqual(new[] { 2 }, segmentedButton.SelectedIndexes, "#2.4");
			Assert.AreEqual(3, selectedIndexesChangedCount, "#2.5");

			item1.Selected = true;

			Assert.AreEqual(0, segmentedButton.SelectedIndex, "#3.1");
			Assert.AreSame(item1, segmentedButton.SelectedItem, "#3.2");
			CollectionAssert.AreEqual(new[] { item1 }, segmentedButton.SelectedItems, "#3.3");
			CollectionAssert.AreEqual(new[] { 0 }, segmentedButton.SelectedIndexes, "#3.4");
			Assert.AreEqual(4, selectedIndexesChangedCount, "#3.5");

			item1.Selected = false;

			Assert.AreEqual(-1, segmentedButton.SelectedIndex, "#4.1");
			Assert.IsNull(segmentedButton.SelectedItem, "#4.2");
			CollectionAssert.AreEqual(new SegmentedItem[0], segmentedButton.SelectedItems, "#4.3");
			CollectionAssert.AreEqual(new int[0], segmentedButton.SelectedIndexes, "#4.4");
			Assert.AreEqual(5, selectedIndexesChangedCount, "#4.5");
		}

		[Test, InvokeOnUI]
		public void AddingAndRemovingSelectedItemShouldChangeSelection()
		{
			var item1 = new ButtonSegmentedItem { Text = "Item1" };
			var item2 = new ButtonSegmentedItem { Text = "Item2", Selected = true };
			var item3 = new ButtonSegmentedItem { Text = "Item3" };

			var segmentedButton = new SegmentedButton
			{
				SelectionMode = SegmentedSelectionMode.Single,
			};
			var selectedIndexesChangedCount = 0;
			segmentedButton.SelectedIndexesChanged += (sender, e) => selectedIndexesChangedCount++;

			// add non-selected item
			segmentedButton.Items.Add(item1);
			Assert.AreEqual(0, selectedIndexesChangedCount, "#1.1");
			Assert.AreEqual(-1, segmentedButton.SelectedIndex, "#1.2");

			// add item that was selected (selection now changed to that item!)
			segmentedButton.Items.Add(item2);
			Assert.AreEqual(1, selectedIndexesChangedCount, "#2.1");
			Assert.AreEqual(1, segmentedButton.SelectedIndex, "#2.2");

			// add another item (no change)
			segmentedButton.Items.Add(item3);
			Assert.AreEqual(1, selectedIndexesChangedCount, "#3.1");
			Assert.AreEqual(1, segmentedButton.SelectedIndex, "#3.2");

			// remove a non-selected item (no change)
			segmentedButton.Items.Remove(item3);
			Assert.AreEqual(1, selectedIndexesChangedCount, "#4.1");
			Assert.AreEqual(1, segmentedButton.SelectedIndex, "#4.2");

			// remove the selected item (change!)
			segmentedButton.Items.Remove(item2);
			Assert.AreEqual(2, selectedIndexesChangedCount, "#5.1");
			Assert.AreEqual(-1, segmentedButton.SelectedIndex, "#5.2");
		}

		[Test, InvokeOnUI]
		public void ChangingModesShouldUpdateSelection()
		{
			var item1 = new ButtonSegmentedItem { Text = "Item1", Selected = true };
			var item2 = new ButtonSegmentedItem { Text = "Item2", Selected = true };
			var item3 = new ButtonSegmentedItem { Text = "Item3", Selected = true };

			var segmentedButton = new SegmentedButton
			{
				SelectionMode = SegmentedSelectionMode.Multiple,
				Items = { item1, item2, item3 }
			};

			var selectedIndexesChangedCount = 0;
			segmentedButton.SelectedIndexesChanged += (sender, e) => selectedIndexesChangedCount++;

			// sanity check, in multiple selection last selected is returned
			Assert.AreEqual(0, segmentedButton.SelectedIndex, "#1.1");
			Assert.AreEqual(item1, segmentedButton.SelectedItem, "#1.2");
			CollectionAssert.AreEquivalent(new[] { 0, 1, 2 }, segmentedButton.SelectedIndexes, "#1.3");
			CollectionAssert.AreEquivalent(new[] { item1, item2, item3 }, segmentedButton.SelectedItems, "#1.4");

			// change mode to single
			segmentedButton.SelectionMode = SegmentedSelectionMode.Single;
			Assert.AreEqual(1, selectedIndexesChangedCount, "#2.1");
			Assert.AreEqual(0, segmentedButton.SelectedIndex, "#2.2");
			Assert.AreEqual(item1, segmentedButton.SelectedItem, "#2.3");
			CollectionAssert.AreEquivalent(new[] { 0 }, segmentedButton.SelectedIndexes, "#2.4");
			CollectionAssert.AreEquivalent(new[] { item1 }, segmentedButton.SelectedItems, "#2.5");

			// accessing selected items shouldn't trigger anything
			Assert.AreEqual(1, selectedIndexesChangedCount, "#3.1");

			// change mode to none
			segmentedButton.SelectionMode = SegmentedSelectionMode.None;
			Assert.AreEqual(2, selectedIndexesChangedCount, "#4.1");
			Assert.AreEqual(-1, segmentedButton.SelectedIndex, "#4.2");
			Assert.AreEqual(null, segmentedButton.SelectedItem, "#4.3");
			CollectionAssert.IsEmpty(segmentedButton.SelectedIndexes, "#4.4");
			CollectionAssert.IsEmpty(segmentedButton.SelectedItems, "#4.5");
		}
	}
}
