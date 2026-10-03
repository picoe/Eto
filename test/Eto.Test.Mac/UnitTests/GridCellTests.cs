using System.Runtime.CompilerServices;
using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
	public class GridCellTests : TestBase
	{
		class Item
		{
			public string Text { get; set; }
			public bool? Check { get; set; }
			public object Combo { get; set; }
			public string ImageText { get; set; }
		}

		static readonly string[] s_comboValues = { "One", "Two", "Three" };

		const int TextColumn = 0;
		const int CheckColumn = 1;
		const int ComboColumn = 2;
		const int ImageTextColumn = 3;

		static List<Item> CreateItems() => Enumerable.Range(0, 5)
			.Select(i => new Item { Text = $"Text {i}", Check = false, Combo = s_comboValues[0], ImageText = $"Image {i}" })
			.ToList();

		static Cell CreateCell(string cellType) => cellType switch
		{
			nameof(TextBoxCell) => new TextBoxCell { Binding = Binding.Property((Item m) => m.Text) },
			nameof(CheckBoxCell) => new CheckBoxCell { Binding = Binding.Property((Item m) => m.Check) },
			nameof(ComboBoxCell) => new ComboBoxCell { DataStore = s_comboValues, Binding = Binding.Property((Item m) => m.Combo) },
			nameof(ImageTextCell) => new ImageTextCell { TextBinding = Binding.Property((Item m) => m.ImageText) },
			nameof(CustomCell) => CreateCustomCell(),
			nameof(DrawableCell) => new DrawableCell(),
			_ => throw new ArgumentOutOfRangeException(nameof(cellType))
		};

		// Binds the cell's control to its args, like RH-89084, so the args stay referenced by the control.
		static CustomCell CreateCustomCell() => new CustomCell
		{
			CreateCell = args =>
			{
				var label = new Label();
				label.Bind(c => c.Enabled, args, a => a.IsSelected, DualBindingMode.OneWay);
				return label;
			},
			ConfigureCell = (args, control) => ((Label)control).Text = (args.Item as Item)?.Text ?? (args.Item as TreeItem)?.Text
		};

		class TreeItem : TreeGridItem
		{
			public string Text { get; set; }
		}

		static GridView CreateGrid(params string[] cellTypes)
		{
			var grid = new GridView { Size = new Size(400, 200) };
			foreach (var cellType in cellTypes)
				grid.Columns.Add(new GridColumn { DataCell = CreateCell(cellType), Editable = true, HeaderText = cellType });
			grid.DataStore = CreateItems();
			return grid;
		}

		static GridView CreateEditGrid() => CreateGrid(nameof(TextBoxCell), nameof(CheckBoxCell), nameof(ComboBoxCell), nameof(ImageTextCell));

		static NSTableView GetTable(GridView grid) => (NSTableView)grid.ControlObject;

		// Makes sure the table has created the visible cell views, as it would when shown on screen.
		static NSView GetCellView(GridView grid, int column, int row)
		{
			var table = GetTable(grid);
			table.LayoutSubtreeIfNeeded();
			return table.GetView(column, row, false);
		}

		[Test]
		public void TextBoxCellEditShouldUpdateItem() => EditTextShouldUpdateItem(TextColumn, item => item.Text);

		[Test]
		public void ImageTextCellEditShouldUpdateItem() => EditTextShouldUpdateItem(ImageTextColumn, item => item.ImageText);

		void EditTextShouldUpdateItem(int column, Func<Item, string> getValue)
		{
			GridViewCellEventArgs editing = null;
			GridViewCellEventArgs edited = null;
			Shown(form => CreateEditGrid(), grid =>
			{
				grid.CellEditing += (sender, e) => editing = e;
				grid.CellEdited += (sender, e) => edited = e;
				var item = ((List<Item>)grid.DataStore)[1];

				grid.BeginEdit(1, column);
				var editor = GetTable(grid).Window.FirstResponder as NSText;
				Assert.That(editor, Is.Not.Null, "Cell should be editing");
				editor.Value = "Edited";
				Assert.That(grid.CommitEdit(), Is.True);

				Assert.That(getValue(item), Is.EqualTo("Edited"));
				Assert.That(editing, Is.Not.Null, "CellEditing should be raised");
				Assert.That(editing.Row, Is.EqualTo(1));
				Assert.That(editing.Column, Is.EqualTo(column));
				Assert.That(edited, Is.Not.Null, "CellEdited should be raised");
				Assert.That(edited.Row, Is.EqualTo(1));
				Assert.That(edited.Column, Is.EqualTo(column));
				Assert.That(edited.Item, Is.SameAs(item));
			});
		}

		[Test]
		public void CheckBoxCellClickShouldUpdateItem()
		{
			GridViewCellEventArgs editing = null;
			GridViewCellEventArgs edited = null;
			Shown(form => CreateEditGrid(), grid =>
			{
				grid.CellEditing += (sender, e) => editing = e;
				grid.CellEdited += (sender, e) => edited = e;
				var item = ((List<Item>)grid.DataStore)[1];

				var button = GetCellView(grid, CheckColumn, 1) as NSButton;
				Assert.That(button, Is.Not.Null, "Check box cell view should exist");
				button.PerformClick(button);

				Assert.That(item.Check, Is.True);
				Assert.That(editing?.Row, Is.EqualTo(1), "CellEditing should be raised");
				Assert.That(edited?.Row, Is.EqualTo(1), "CellEdited should be raised");
				Assert.That(edited.Column, Is.EqualTo(CheckColumn));
				Assert.That(edited.Item, Is.SameAs(item));
			});
		}

		[Test]
		public void ComboBoxCellSelectShouldUpdateItem()
		{
			GridViewCellEventArgs editing = null;
			GridViewCellEventArgs edited = null;
			Shown(form => CreateEditGrid(), grid =>
			{
				grid.CellEditing += (sender, e) => editing = e;
				grid.CellEdited += (sender, e) => edited = e;
				var item = ((List<Item>)grid.DataStore)[1];

				var popup = GetCellView(grid, ComboColumn, 1) as NSPopUpButton;
				Assert.That(popup, Is.Not.Null, "Combo box cell view should exist");
				// Choose an item the way the menu does, without showing the menu.
				popup.SelectItem(2);
				popup.SendAction(popup.Action, popup.Target);

				Assert.That(item.Combo, Is.EqualTo(s_comboValues[2]));
				Assert.That(editing?.Row, Is.EqualTo(1), "CellEditing should be raised");
				Assert.That(edited?.Row, Is.EqualTo(1), "CellEdited should be raised");
				Assert.That(edited.Column, Is.EqualTo(ComboColumn));
				Assert.That(edited.Item, Is.SameAs(item));
			});
		}

		[Test]
		public void CustomCellArgsShouldHaveGrid()
		{
			Grid configureGrid = null;
			Shown(form =>
			{
				var grid = new GridView { Size = new Size(400, 200) };
				var cell = CreateCustomCell();
				var configure = cell.ConfigureCell;
				cell.ConfigureCell = (args, control) =>
				{
					configureGrid ??= args.Grid;
					configure(args, control);
				};
				grid.Columns.Add(new GridColumn { DataCell = cell });
				grid.DataStore = CreateItems();
				return grid;
			}, grid =>
			{
				Assert.That(GetCellView(grid, 0, 0), Is.Not.Null, "Cell view should be created when shown");
				Assert.That(configureGrid, Is.SameAs(grid));
			});
		}

		[TestCase(false)]
		[TestCase(true)]
		public void CustomCellItemsShouldBeCollectedWhenDataStoreIsCleared(bool treeGrid)
		{
			// RH-89084: the grid stays alive, but nothing in it should keep the old items.
			WeakReference itemReference = null;
			Shown(form => CreateCustomCellGrid(treeGrid, out itemReference), grid =>
			{
				var table = (NSTableView)grid.ControlObject;
				table.LayoutSubtreeIfNeeded();
				Assert.That(table.GetView(0, 0, false), Is.Not.Null, "Cell view should be created when shown");

				if (grid is TreeGridView treeGridView)
					treeGridView.DataStore = null;
				else
					((GridView)grid).DataStore = null;

				for (int i = 0; itemReference.IsAlive && i < 50; i++)
				{
					GC.Collect();
					GC.WaitForPendingFinalizers();
					Application.Instance.RunIteration();
				}
				Assert.That(itemReference.IsAlive, Is.False, "Item should be collected");
			});
		}

		// RH-89084: same setup as the script attached to the issue.
		class SelectableDrawable : Drawable
		{
			public bool IsSelected { get; set; }
		}

		class SelectableCell : CustomCell
		{
			protected sealed override Control OnCreateCell(CellEventArgs args)
			{
				var drawable = new SelectableDrawable();
				drawable.Bind(d => d.IsSelected, args, a => a.IsSelected, DualBindingMode.OneWay);
				return drawable;
			}
		}

		class LookForThis : TreeGridItem
		{
		}

		[Test]
		public void TreeGridViewShouldNotKeepItemsAfterDataStoreIsCleared()
		{
			WeakReference itemReference = null;
			Shown(form => CreateSelectableCellTree(out itemReference), tree =>
			{
				var outline = (NSTableView)tree.ControlObject;
				outline.LayoutSubtreeIfNeeded();
				Assert.That(outline.GetView(0, 0, false), Is.Not.Null, "Cell view should be created when shown");

				tree.DataStore = default;
				tree.ReloadData();

				for (int i = 0; itemReference.IsAlive && i < 50; i++)
				{
					GC.Collect();
					GC.WaitForPendingFinalizers();
					Application.Instance.RunIteration();
				}
				Assert.That(itemReference.IsAlive, Is.False, "Item should be collected");
			});
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static TreeGridView CreateSelectableCellTree(out WeakReference itemReference)
		{
			var items = new TreeGridItemCollection { new LookForThis() };
			itemReference = new WeakReference(items[0]);
			var tree = new TreeGridView { ShowHeader = false, Size = new Size(400, 400) };
			tree.Columns.Add(new GridColumn { DataCell = new SelectableCell() });
			tree.DataStore = items;
			tree.ReloadData();
			return tree;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static Grid CreateCustomCellGrid(bool treeGrid, out WeakReference itemReference)
		{
			Grid grid;
			if (treeGrid)
			{
				var items = new TreeGridItemCollection { new TreeItem { Text = "Item" } };
				itemReference = new WeakReference(items[0]);
				grid = new TreeGridView { DataStore = items };
			}
			else
			{
				var items = new List<Item> { new Item { Text = "Item" } };
				itemReference = new WeakReference(items[0]);
				grid = new GridView { DataStore = items };
			}
			grid.Size = new Size(400, 200);
			grid.Columns.Add(new GridColumn { DataCell = CreateCustomCell(), AutoSize = true });
			return grid;
		}

		[TestCase(nameof(TextBoxCell))]
		[TestCase(nameof(CheckBoxCell))]
		[TestCase(nameof(ComboBoxCell))]
		[TestCase(nameof(ImageTextCell))]
		[TestCase(nameof(CustomCell))]
		[TestCase(nameof(DrawableCell))]
		public void GridShouldBeCollectedAfterItsCellsWereShown(string cellType)
		{
			// Cocoa keeps the cell views (and their managed peers) alive, so if they reference the grid it
			// can never be collected when it is removed without being disposed.
			WeakReference gridReference = null;
			Shown(form => new Panel { Size = new Size(400, 200) }, holder =>
			{
				gridReference = ShowAndRemoveGrid(holder, cellType);
			});

			for (int i = 0; gridReference.IsAlive && i < 50; i++)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				// Native views are released on the main thread, one level of the view tree at a time.
				Invoke(() => Application.Instance.RunIteration());
				Thread.Sleep(10);
			}
			Assert.That(gridReference.IsAlive, Is.False, "Grid should be collected");
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static WeakReference ShowAndRemoveGrid(Panel holder, string cellType) => ShowAndRemoveGrid(holder, CreateCell(cellType));

		[MethodImpl(MethodImplOptions.NoInlining)]
		static WeakReference ShowAndRemoveGrid(Panel holder, Cell cell)
		{
			var grid = new GridView { Size = new Size(400, 200) };
			grid.Columns.Add(new GridColumn { DataCell = cell, Editable = true });
			grid.DataStore = CreateItems();
			holder.Content = grid;
			holder.ParentWindow.UpdateLayout();
			Assert.That(GetCellView(grid, 0, 0), Is.Not.Null, "Cell view should be created when shown");
			// Remove without disposing, like a page that is dropped while its view is still around.
			holder.Content = null;
			return new WeakReference(grid);
		}
	}
}
