using NUnit.Framework;
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

				Assert.That(itemExpected.Selected, Is.True, "#1.1");

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

			Assert.That(selectedIndexesChangedCount, Is.EqualTo(0), "#2.1");
			Assert.That(itemClickCount, Is.EqualTo(1), "#2.2");
			Assert.That(clickCount, Is.EqualTo(1), "#2.3");

			Assert.That(itemExpected, Is.Not.Null, "#3.1");
			Assert.That(itemExpected, Is.SameAs(itemClicked), "#3.2");
			Assert.That(itemExpected, Is.SameAs(itemItemClicked), "#3.3");
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
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(1), "#1.1");
			segmentedButton.Items.Add(item2);
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(2), "#1.2");
			segmentedButton.Items.Add(item3);
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(3), "#1.3");

			Assert.That(segmentedButton.SelectedIndex, Is.EqualTo(2), "#2.1");
			Assert.That(item3, Is.SameAs(segmentedButton.SelectedItem), "#2.2");
			Assert.That(segmentedButton.SelectedItems, Is.EqualTo(new[] { item3 }), "#2.3");
			Assert.That(segmentedButton.SelectedIndexes, Is.EqualTo(new[] { 2 }), "#2.4");
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(3), "#2.5");

			item1.Selected = true;

			Assert.That(segmentedButton.SelectedIndex, Is.EqualTo(0), "#3.1");
			Assert.That(item1, Is.SameAs(segmentedButton.SelectedItem), "#3.2");
			Assert.That(segmentedButton.SelectedItems, Is.EqualTo(new[] { item1 }), "#3.3");
			Assert.That(segmentedButton.SelectedIndexes, Is.EqualTo(new[] { 0 }), "#3.4");
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(4), "#3.5");

			item1.Selected = false;

			Assert.That(segmentedButton.SelectedIndex, Is.EqualTo(-1), "#4.1");
			Assert.That(segmentedButton.SelectedItem, Is.Null, "#4.2");
			Assert.That(segmentedButton.SelectedItems, Is.EqualTo(new SegmentedItem[0]), "#4.3");
			Assert.That(segmentedButton.SelectedIndexes, Is.EqualTo(new int[0]), "#4.4");
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(5), "#4.5");
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
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(0), "#1.1");
			Assert.That(segmentedButton.SelectedIndex, Is.EqualTo(-1), "#1.2");

			// add item that was selected (selection now changed to that item!)
			segmentedButton.Items.Add(item2);
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(1), "#2.1");
			Assert.That(segmentedButton.SelectedIndex, Is.EqualTo(1), "#2.2");

			// add another item (no change)
			segmentedButton.Items.Add(item3);
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(1), "#3.1");
			Assert.That(segmentedButton.SelectedIndex, Is.EqualTo(1), "#3.2");

			// remove a non-selected item (no change)
			segmentedButton.Items.Remove(item3);
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(1), "#4.1");
			Assert.That(segmentedButton.SelectedIndex, Is.EqualTo(1), "#4.2");

			// remove the selected item (change!)
			segmentedButton.Items.Remove(item2);
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(2), "#5.1");
			Assert.That(segmentedButton.SelectedIndex, Is.EqualTo(-1), "#5.2");
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
			Assert.That(segmentedButton.SelectedIndex, Is.EqualTo(0), "#1.1");
			Assert.That(segmentedButton.SelectedItem, Is.EqualTo(item1), "#1.2");
			Assert.That(segmentedButton.SelectedIndexes, Is.EquivalentTo(new[] { 0, 1, 2 }), "#1.3");
			Assert.That(segmentedButton.SelectedItems, Is.EquivalentTo(new[] { item1, item2, item3 }), "#1.4");

			// change mode to single
			segmentedButton.SelectionMode = SegmentedSelectionMode.Single;
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(1), "#2.1");
			Assert.That(segmentedButton.SelectedIndex, Is.EqualTo(0), "#2.2");
			Assert.That(segmentedButton.SelectedItem, Is.EqualTo(item1), "#2.3");
			Assert.That(segmentedButton.SelectedIndexes, Is.EquivalentTo(new[] { 0 }), "#2.4");
			Assert.That(segmentedButton.SelectedItems, Is.EquivalentTo(new[] { item1 }), "#2.5");

			// accessing selected items shouldn't trigger anything
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(1), "#3.1");

			// change mode to none
			segmentedButton.SelectionMode = SegmentedSelectionMode.None;
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(2), "#4.1");
			Assert.That(segmentedButton.SelectedIndex, Is.EqualTo(-1), "#4.2");
			Assert.That(segmentedButton.SelectedItem, Is.EqualTo(null), "#4.3");
			Assert.That(segmentedButton.SelectedIndexes, Is.Empty, "#4.4");
			Assert.That(segmentedButton.SelectedItems, Is.Empty, "#4.5");
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

			Assert.That(control.SelectedIndexChangedCount, Is.EqualTo(0), "#1");

			control.SelectedIndex = 0;

			Assert.That(control.SelectedIndexChangedCount, Is.EqualTo(1), "#2");
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

			Assert.That(selectedIndexesChangedCount, Is.EqualTo(0), "#1");

			control.SelectedIndex = 0;
			Assert.That(control.SelectedIndex, Is.EqualTo(-1), "#2.1");
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(0), "#2.2");
			Assert.That(control.SelectedIndexes, Is.Empty, "#2.3");

			control.SelectedIndexes = new[] { 1, 2 };
			Assert.That(control.SelectedIndex, Is.EqualTo(-1), "#3.1");
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(0), "#3.2");
			Assert.That(control.SelectedIndexes, Is.Empty, "#3.3");
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

			Assert.That(itemClickWasRaised, Is.EqualTo(1), "#1.1"); // ensure user actually clicked an item.
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(0), "#1.2");
			Assert.That(selectedItems, Is.Empty, "#1.3");
			Assert.That(selectedIndex, Is.EqualTo(-1), "#1.4");

			// check item events
			Assert.That(itemWasClicked, Is.EqualTo(1), "#2.1");
			Assert.That(itemSelectedWasChanged, Is.EqualTo(0), "#2.2");
			Assert.That(itemIsSelected, Is.False, "#2.3");
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
				Assert.That(model.SelectedIndexChangedCount, Is.EqualTo(1), "#1.1"); // set when binding

				control.SelectedIndexesChanged += (sender, e) => selectedIndexesChangedCount++;
				control.SelectedIndexChanged += (sender, e) => selectedIndexChangedCount++;
				control.Items.Add("Item1");
				var item2 = new ButtonSegmentedItem { Text = "Click Me" };
				item2.BindDataContext(r => r.Selected, (SegmentedModel m) => m.ItemIsSelected, DualBindingMode.OneWayToSource);
				Assert.That(model.ItemIsSelectedChangedCount, Is.EqualTo(0), "#1.2");
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
				Assert.That(itemClickWasRaised, Is.EqualTo(1), "#2.1"); // ensure user actually clicked an item.
				Assert.That(model.SelectedIndex, Is.EqualTo(selectedIndex), "#2.2");

				// check events on the item itself
				Assert.That(itemWasClicked, Is.EqualTo(1), "#2.3");

				if (selectionMode == SegmentedSelectionMode.Multiple)
				{
					if (initiallySelected)
					{
						Assert.That(selectedIndexChangedCount, Is.EqualTo(2), "#3.1.1");
						Assert.That(selectedIndexesChangedCount, Is.EqualTo(2), "#3.1.2");
						Assert.That(selectedIndex >= 0, Is.False, "#3.1.3");
						Assert.That(model.SelectedIndexChangedCount, Is.EqualTo(3), "#3.1.4"); // one for binding, one when item is added, and one when it actually changes.
						Assert.That(model.ItemIsSelected, Is.False, "#3.1.5");
						Assert.That(model.ItemIsSelectedChangedCount, Is.EqualTo(3), "#3.1.6"); // one for binding, one when it is set, and one when it actually changes.
					}
					else
					{
						Assert.That(selectedIndexChangedCount, Is.EqualTo(1), "#3.2.1");
						Assert.That(selectedIndexesChangedCount, Is.EqualTo(1), "#3.2.2");
						Assert.That(selectedIndex >= 0, Is.True, "#3.2.3");
						Assert.That(model.SelectedIndexChangedCount, Is.EqualTo(2), "#3.2.4"); // one for binding, one when it actually changes.
						Assert.That(model.ItemIsSelected, Is.True, "#3.2.5");
						Assert.That(model.ItemIsSelectedChangedCount, Is.EqualTo(2), "#3.2.6"); // one for binding, and one when it actually changes.
					}

					Assert.That(itemSelectedWasChanged, Is.EqualTo(1), "#3.3.1");
					Assert.That(itemIsSelected, Is.Not.EqualTo(initiallySelected), "#3.3.2");
				}
				else
				{
					Assert.That(selectedIndexChangedCount, Is.EqualTo(1), "#4.1.1");
					Assert.That(selectedIndexesChangedCount, Is.EqualTo(1), "#4.1.2");
					Assert.That(selectedIndex >= 0, Is.True, "#4.1.3");
					Assert.That(model.SelectedIndexChangedCount, Is.EqualTo(2), "#4.1.4"); // one for binding, one when it actually changes.
					Assert.That(model.ItemIsSelected, Is.True, "#4.1.5");
					Assert.That(model.ItemIsSelectedChangedCount, Is.EqualTo(2), "#4.1.6"); // set when binding

					if (initiallySelected)
						Assert.That(itemSelectedWasChanged, Is.EqualTo(0), "#4.2.1");
					else
						Assert.That(itemSelectedWasChanged, Is.EqualTo(1), "#4.2.2");

					Assert.That(itemIsSelected, Is.True, "#4.2.3");
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
			Assert.That(control.SelectedIndexes, Is.Empty, "#1.1");
			Assert.That(control.SelectedIndex, Is.EqualTo(-1), "#1.2");
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(0), "#1.3");
			Assert.That(item1.Selected, Is.False, "#1.4");
			Assert.That(item2.Selected, Is.True, "#1.5");
			Assert.That(item3.Selected, Is.False, "#1.6");
			Assert.That(item1SelectedChanged, Is.EqualTo(0), "#1.7.1");
			Assert.That(item2SelectedChanged, Is.EqualTo(1), "#1.7.2");
			Assert.That(item3SelectedChanged, Is.EqualTo(0), "#1.7.3");

			control.Items.Add(item1);
			Assert.That(control.SelectedIndexes, Is.Empty, "#2.1");
			Assert.That(control.SelectedIndex, Is.EqualTo(-1), "#2.2");
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(0), "#2.3");
			Assert.That(item1.Selected, Is.False, "#2.4");
			Assert.That(item2.Selected, Is.True, "#2.5");
			Assert.That(item3.Selected, Is.False, "#2.6");
			Assert.That(item1SelectedChanged, Is.EqualTo(0), "#2.7.1");
			Assert.That(item2SelectedChanged, Is.EqualTo(1), "#2.7.2");
			Assert.That(item3SelectedChanged, Is.EqualTo(0), "#2.7.3");

			control.Items.Add(item2);
			Assert.That(control.SelectedIndexes, Is.EqualTo(new[] { 1 }), "#3.1");
			Assert.That(control.SelectedIndex, Is.EqualTo(1), "#3.2");
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(1), "#3.3");
			Assert.That(item1.Selected, Is.False, "#3.4");
			Assert.That(item2.Selected, Is.True, "#3.5");
			Assert.That(item3.Selected, Is.False, "#3.6");
			Assert.That(item1SelectedChanged, Is.EqualTo(0), "#3.7.1");
			Assert.That(item2SelectedChanged, Is.EqualTo(1), "#3.7.2");
			Assert.That(item3SelectedChanged, Is.EqualTo(0), "#3.7.3");

			control.Items.Add(item3);
			// no change
			Assert.That(control.SelectedIndexes, Is.EqualTo(new[] { 1 }), "#4.1");
			Assert.That(control.SelectedIndex, Is.EqualTo(1), "#4.2");
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(1), "#4.3");
			Assert.That(item1.Selected, Is.False, "#4.4");
			Assert.That(item2.Selected, Is.True, "#4.5");
			Assert.That(item3.Selected, Is.False, "#4.6");
			Assert.That(item1SelectedChanged, Is.EqualTo(0), "#4.7.1");
			Assert.That(item2SelectedChanged, Is.EqualTo(1), "#4.7.2");
			Assert.That(item3SelectedChanged, Is.EqualTo(0), "#4.7.3");

			control.SelectAll();
			Assert.That(control.SelectedIndexes, Is.EquivalentTo(new[] { 0, 1, 2 }), "#5.1");
			Assert.That(control.SelectedIndex, Is.EqualTo(0), "#5.2");
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(2), "#5.3");
			Assert.That(item1.Selected, Is.True, "#5.4");
			Assert.That(item2.Selected, Is.True, "#5.5");
			Assert.That(item3.Selected, Is.True, "#5.6");
			Assert.That(item1SelectedChanged, Is.EqualTo(1), "#5.7.1");
			Assert.That(item2SelectedChanged, Is.EqualTo(1), "#5.7.2");
			Assert.That(item3SelectedChanged, Is.EqualTo(1), "#5.7.3");


			control.ClearSelection();
			Assert.That(control.SelectedIndexes, Is.Empty, "#6.1");
			Assert.That(control.SelectedIndex, Is.EqualTo(-1), "#6.2");
			Assert.That(selectedIndexesChangedCount, Is.EqualTo(3), "#6.3");
			Assert.That(item1.Selected, Is.False, "#6.4");
			Assert.That(item2.Selected, Is.False, "#6.5");
			Assert.That(item3.Selected, Is.False, "#6.6");
			Assert.That(item1SelectedChanged, Is.EqualTo(2), "#6.7.1");
			Assert.That(item2SelectedChanged, Is.EqualTo(2), "#6.7.2");
			Assert.That(item3SelectedChanged, Is.EqualTo(2), "#6.7.3");
		}
		
		[Test]
		[InvokeOnUI]
		public void InsertingItemsShouldWork()
		{
			var segmentedButton = new SegmentedButton();
			segmentedButton.Items.Add(new ButtonSegmentedItem { Text = "Item 1" });
			segmentedButton.Items.Add(new ButtonSegmentedItem { Text = "Item 2" });
			segmentedButton.Items.Add(new ButtonSegmentedItem { Text = "Item 3" });

			// insert at beginning
			segmentedButton.Items.Insert(0, new ButtonSegmentedItem { Text = "Item Beginning" });
			// insert at middle
			segmentedButton.Items.Insert(1, new ButtonSegmentedItem { Text = "Item Middle" });
			// insert at end
			segmentedButton.Items.Insert(segmentedButton.Items.Count, new ButtonSegmentedItem { Text = "Item End" });
		}
	}
}