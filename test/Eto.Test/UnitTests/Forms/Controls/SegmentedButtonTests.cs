using System;
using NUnit.Framework;
using Eto.Forms;
using System.Linq;

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

		class SegmentedButtonSubclass : SegmentedButton
		{
			public int SelectedIndexChangedCount { get; set; }

			protected override void OnSelectedIndexChanged(EventArgs e)
			{
				base.OnSelectedIndexChanged(e);
				SelectedIndexChangedCount++;
			}
		}

		[Test, InvokeOnUI]
		public void SelectedIndexOverrideShouldTriggerEvent()
		{
			var control = new SegmentedButtonSubclass { Items = { "Item1", "Item2", "Item3" }, SelectionMode = SegmentedSelectionMode.Single };

			Assert.AreEqual(0, control.SelectedIndexChangedCount, "#1");

			control.SelectedIndex = 0;

			Assert.AreEqual(1, control.SelectedIndexChangedCount, "#2");
		}

		[Test, InvokeOnUI]
		public void ChangingSelectionWhenModeIsNoneShouldNotRaiseChangedEvents()
		{
			var control = new SegmentedButton();
			int selectedIndexesChangedCount = 0;
			control.SelectedIndexesChanged += (sender, e) => selectedIndexesChangedCount++;
			control.Items.Add("Item1");
			control.Items.Add("Item2");
			control.Items.Add("Item3");

			Assert.AreEqual(0, selectedIndexesChangedCount, "#1");

			control.SelectedIndex = 0;
			Assert.AreEqual(-1, control.SelectedIndex, "#2.1");
			Assert.AreEqual(0, selectedIndexesChangedCount, "#2.2");
			CollectionAssert.IsEmpty(control.SelectedIndexes, "#2.3");

			control.SelectedIndexes = new[] { 1, 2 };
			Assert.AreEqual(-1, control.SelectedIndex, "#3.1");
			Assert.AreEqual(0, selectedIndexesChangedCount, "#3.2");
			CollectionAssert.IsEmpty(control.SelectedIndexes, "#3.3");
		}

		[Test, ManualTest]
		public void ClickingAnItemWhenModeIsNoneShouldNotRaiseChangedEvents()
		{
			int selectedIndexesChangedCount = 0;
			int itemClickWasRaised = 0;
			int itemWasClicked = 0;
			int itemSelectedWasChanged = 0;
			bool itemIsSelected = false;
			int[] selectedItems = null;
			int selectedIndex = 0;

			ManualForm("Click on the Click Me item", form =>
			{
				var control = new SegmentedButton();
				control.SelectedIndexesChanged += (sender, e) => selectedIndexesChangedCount++;
				control.Items.Add("Item1");
				var item2 = new ButtonSegmentedItem { Text = "Click Me" };
				item2.Click += (sender, e) => itemWasClicked++;
				item2.SelectedChanged += (sender, e) => itemSelectedWasChanged++;
				control.Items.Add(item2);
				control.Items.Add("Item3");

				// async in case code runs after this event.
				control.ItemClick += (sender, e) =>
				{
					itemClickWasRaised++;
					itemIsSelected = item2.Selected;
					selectedIndex = control.SelectedIndex;
					selectedItems = control.SelectedIndexes?.ToArray();
					Application.Instance.AsyncInvoke(form.Close);
				};
				return control;
			}, allowPass: false);

			Assert.AreEqual(1, itemClickWasRaised, "#1.1"); // ensure user actually clicked an item.
			Assert.AreEqual(0, selectedIndexesChangedCount, "#1.2");
			CollectionAssert.IsEmpty(selectedItems, "#1.3");
			Assert.AreEqual(-1, selectedIndex, "#1.4");

			// check item events
			Assert.AreEqual(1, itemWasClicked, "#2.1");
			Assert.AreEqual(0, itemSelectedWasChanged, "#2.2");
			Assert.IsFalse(itemIsSelected, "#2.3");
		}

		class SegmentedModel
		{
			int selectedIndex;
			bool itemIsSelected;

			public int SelectedIndexChangedCount { get; set; }
			public int SelectedIndex
			{
				get => selectedIndex;
				set
				{
					selectedIndex = value;
					SelectedIndexChangedCount++;
				}
			}

			public int ItemIsSelectedChangedCount { get; set; }
			public bool ItemIsSelected
			{
				get => itemIsSelected;
				set
				{
					itemIsSelected = value;
					ItemIsSelectedChangedCount++;
				}
			}
		}

		[TestCase(SegmentedSelectionMode.Single, true)]
		[TestCase(SegmentedSelectionMode.Single, false)]
		[TestCase(SegmentedSelectionMode.Multiple, true)]
		[TestCase(SegmentedSelectionMode.Multiple, false)]
		[ManualTest]
		public void ClickingAnItemShouldRaiseChangedEvents(SegmentedSelectionMode selectionMode, bool initiallySelected)
		{
			int selectedIndexesChangedCount = 0;
			int selectedIndexChangedCount = 0;
			int itemClickWasRaised = 0;

			bool itemIsSelected = false;
			int itemWasClicked = 0;
			int itemSelectedWasChanged = 0;

			int selectedIndex = -1;
			var model = new SegmentedModel();

			ManualForm("Click on the Click Me item", form =>
			{
				var control = new SegmentedButton();
				control.DataContext = model;
				control.SelectionMode = selectionMode;

				control.BindDataContext(c => c.SelectedIndex, (SegmentedModel m) => m.SelectedIndex, DualBindingMode.OneWayToSource);
				Assert.AreEqual(1, model.SelectedIndexChangedCount, "#1.1"); // set when binding

				control.SelectedIndexesChanged += (sender, e) => selectedIndexesChangedCount++;
				control.SelectedIndexChanged += (sender, e) => selectedIndexChangedCount++;
				control.Items.Add("Item1");
				var item2 = new ButtonSegmentedItem { Text = "Click Me" };
				item2.BindDataContext(r => r.Selected, (SegmentedModel m) => m.ItemIsSelected, DualBindingMode.OneWayToSource);
				Assert.AreEqual(0, model.ItemIsSelectedChangedCount, "#1.2");
				item2.Selected = initiallySelected;
				item2.Click += (sender, e) =>
				{
					itemWasClicked++;
					itemIsSelected = item2.Selected;
				};
				item2.SelectedChanged += (sender, e) =>
				{
					itemSelectedWasChanged++;
				};
				control.Items.Add(item2);
				control.Items.Add("Item3");

				// async in case code runs after this event.
				control.ItemClick += (sender, e) =>
				{
					itemClickWasRaised++;
					selectedIndex = control.SelectedIndex;
					Application.Instance.AsyncInvoke(form.Close);
				};
				return control;
			}, allowPass: false);

			Assert.Multiple(() =>
			{
				// check events on the segmented button control
				Assert.AreEqual(1, itemClickWasRaised, "#2.1"); // ensure user actually clicked an item.
				Assert.AreEqual(selectedIndex, model.SelectedIndex, "#2.2");

				// check events on the item itself
				Assert.AreEqual(1, itemWasClicked, "#2.3");

				if (selectionMode == SegmentedSelectionMode.Multiple)
				{
					if (initiallySelected)
					{
						Assert.AreEqual(2, selectedIndexChangedCount, "#3.1.1");
						Assert.AreEqual(2, selectedIndexesChangedCount, "#3.1.2");
						Assert.IsFalse(selectedIndex >= 0, "#3.1.3");
						Assert.AreEqual(3, model.SelectedIndexChangedCount, "#3.1.4"); // one for binding, one when item is added, and one when it actually changes.
						Assert.IsFalse(model.ItemIsSelected, "#3.1.5");
						Assert.AreEqual(3, model.ItemIsSelectedChangedCount, "#3.1.6"); // one for binding, one when it is set, and one when it actually changes.
					}
					else
					{
						Assert.AreEqual(1, selectedIndexChangedCount, "#3.2.1");
						Assert.AreEqual(1, selectedIndexesChangedCount, "#3.2.2");
						Assert.IsTrue(selectedIndex >= 0, "#3.2.3");
						Assert.AreEqual(2, model.SelectedIndexChangedCount, "#3.2.4"); // one for binding, one when it actually changes.
						Assert.IsTrue(model.ItemIsSelected, "#3.2.5");
						Assert.AreEqual(2, model.ItemIsSelectedChangedCount, "#3.2.6"); // one for binding, and one when it actually changes.
					}

					Assert.AreEqual(1, itemSelectedWasChanged, "#3.3.1");
					Assert.AreNotEqual(itemIsSelected, initiallySelected, "#3.3.2");
				}
				else
				{
					Assert.AreEqual(1, selectedIndexChangedCount, "#4.1.1");
					Assert.AreEqual(1, selectedIndexesChangedCount, "#4.1.2");
					Assert.IsTrue(selectedIndex >= 0, "#4.1.3");
					Assert.AreEqual(2, model.SelectedIndexChangedCount, "#4.1.4"); // one for binding, one when it actually changes.
					Assert.IsTrue(model.ItemIsSelected, "#4.1.5");
					Assert.AreEqual(2, model.ItemIsSelectedChangedCount, "#4.1.6"); // set when binding

					if (initiallySelected)
						Assert.AreEqual(0, itemSelectedWasChanged, "#4.2.1");
					else
						Assert.AreEqual(1, itemSelectedWasChanged, "#4.2.2");

					Assert.IsTrue(itemIsSelected, "#4.2.3");
				}
			});
		}

		[Test, InvokeOnUI]
		public void SelectAllAndClearSelectionShouldTriggerSelectedChanges()
		{
			int selectedIndexesChangedCount = 0;
			int item1SelectedChanged = 0;
			int item2SelectedChanged = 0;
			int item3SelectedChanged = 0;

			var control = new SegmentedButton { SelectionMode = SegmentedSelectionMode.Multiple };

			control.SelectedIndexesChanged += (sender, e) => selectedIndexesChangedCount++;

			var item1 = new ButtonSegmentedItem { Text = "Item1" };
			item1.SelectedChanged += (sender, e) => item1SelectedChanged++;
			var item2 = new ButtonSegmentedItem { Text = "Item2" };
			item2.SelectedChanged += (sender, e) => item2SelectedChanged++;
			item2.Selected = true;
			var item3 = new ButtonSegmentedItem { Text = "Item3" };
			item3.SelectedChanged += (sender, e) => item3SelectedChanged++;
			CollectionAssert.IsEmpty(control.SelectedIndexes, "#1.1");
			Assert.AreEqual(-1, control.SelectedIndex, "#1.2");
			Assert.AreEqual(0, selectedIndexesChangedCount, "#1.3");
			Assert.IsFalse(item1.Selected, "#1.4");
			Assert.IsTrue(item2.Selected, "#1.5");
			Assert.IsFalse(item3.Selected, "#1.6");
			Assert.AreEqual(0, item1SelectedChanged, "#1.7.1");
			Assert.AreEqual(1, item2SelectedChanged, "#1.7.2");
			Assert.AreEqual(0, item3SelectedChanged, "#1.7.3");

			control.Items.Add(item1);
			CollectionAssert.IsEmpty(control.SelectedIndexes, "#2.1");
			Assert.AreEqual(-1, control.SelectedIndex, "#2.2");
			Assert.AreEqual(0, selectedIndexesChangedCount, "#2.3");
			Assert.IsFalse(item1.Selected, "#2.4");
			Assert.IsTrue(item2.Selected, "#2.5");
			Assert.IsFalse(item3.Selected, "#2.6");
			Assert.AreEqual(0, item1SelectedChanged, "#2.7.1");
			Assert.AreEqual(1, item2SelectedChanged, "#2.7.2");
			Assert.AreEqual(0, item3SelectedChanged, "#2.7.3");

			control.Items.Add(item2);
			CollectionAssert.AreEqual(new[] { 1 }, control.SelectedIndexes, "#3.1");
			Assert.AreEqual(1, control.SelectedIndex, "#3.2");
			Assert.AreEqual(1, selectedIndexesChangedCount, "#3.3");
			Assert.IsFalse(item1.Selected, "#3.4");
			Assert.IsTrue(item2.Selected, "#3.5");
			Assert.IsFalse(item3.Selected, "#3.6");
			Assert.AreEqual(0, item1SelectedChanged, "#3.7.1");
			Assert.AreEqual(1, item2SelectedChanged, "#3.7.2");
			Assert.AreEqual(0, item3SelectedChanged, "#3.7.3");

			control.Items.Add(item3);
			// no change
			CollectionAssert.AreEqual(new[] { 1 }, control.SelectedIndexes, "#4.1");
			Assert.AreEqual(1, control.SelectedIndex, "#4.2");
			Assert.AreEqual(1, selectedIndexesChangedCount, "#4.3");
			Assert.IsFalse(item1.Selected, "#4.4");
			Assert.IsTrue(item2.Selected, "#4.5");
			Assert.IsFalse(item3.Selected, "#4.6");
			Assert.AreEqual(0, item1SelectedChanged, "#4.7.1");
			Assert.AreEqual(1, item2SelectedChanged, "#4.7.2");
			Assert.AreEqual(0, item3SelectedChanged, "#4.7.3");

			control.SelectAll();
			CollectionAssert.AreEquivalent(new[] { 0, 1, 2 }, control.SelectedIndexes, "#5.1");
			Assert.AreEqual(0, control.SelectedIndex, "#5.2");
			Assert.AreEqual(2, selectedIndexesChangedCount, "#5.3");
			Assert.IsTrue(item1.Selected, "#5.4");
			Assert.IsTrue(item2.Selected, "#5.5");
			Assert.IsTrue(item3.Selected, "#5.6");
			Assert.AreEqual(1, item1SelectedChanged, "#5.7.1");
			Assert.AreEqual(1, item2SelectedChanged, "#5.7.2");
			Assert.AreEqual(1, item3SelectedChanged, "#5.7.3");


			control.ClearSelection();
			CollectionAssert.IsEmpty(control.SelectedIndexes, "#6.1");
			Assert.AreEqual(-1, control.SelectedIndex, "#6.2");
			Assert.AreEqual(3, selectedIndexesChangedCount, "#6.3");
			Assert.IsFalse(item1.Selected, "#6.4");
			Assert.IsFalse(item2.Selected, "#6.5");
			Assert.IsFalse(item3.Selected, "#6.6");
			Assert.AreEqual(2, item1SelectedChanged, "#6.7.1");
			Assert.AreEqual(2, item2SelectedChanged, "#6.7.2");
			Assert.AreEqual(2, item3SelectedChanged, "#6.7.3");
		}
	}
}