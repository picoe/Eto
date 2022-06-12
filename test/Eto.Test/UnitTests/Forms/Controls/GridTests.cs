using NUnit.Framework;
using Eto.Forms;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Test.UnitTests.Forms.Controls
{
	public abstract class GridTests<T> : TestBase
		where T : Grid, new()
	{
		class GridTestItem : TreeGridItem
		{
			public string Text { get; set; }

			public override string ToString() => Text ?? base.ToString();
		}

		[Test, ManualTest]
		public void BeginEditShoudWorkOnCustomCells()
		{
			ManualForm("The custom cell should go in edit mode when clicking the BeginEdit button", form =>
			{
				var grid = new T();
				grid.ShowHeader = true;
				grid.AllowMultipleSelection = true;

				string CellInfo(GridViewCellEventArgs e) => $"Row: {e.Row}, Column: {e.Column}";
				string CellEditInfo(CellEventArgs e) => $"Row: {e.Row}";
				void AddLogging(CustomCell cell)
				{
					cell.BeginEdit += (sender, e) => Log.Write(sender, $"BeginEdit {CellEditInfo(e)}, Grid.IsEditing: {grid.IsEditing}");
					cell.CommitEdit += (sender, e) => Log.Write(sender, $"CommitEdit {CellEditInfo(e)}, Grid.IsEditing: {grid.IsEditing}");
					cell.CancelEdit += (sender, e) => Log.Write(sender, $"CancelEdit {CellEditInfo(e)}, Grid.IsEditing: {grid.IsEditing}");

					if (!CustomCell.SupportsControlView)
					{
						cell.GetPreferredWidth = args => 100;
						cell.Paint += (sender, e) =>
						{
							e.Graphics.DrawText(SystemFonts.Default(), Brushes.Black, e.ClipRectangle, "Cell", alignment: FormattedTextAlignment.Center);
						};
					}

				}

				grid.CellEditing += (sender, e) => Log.Write(sender, $"CellEditing {CellInfo(e)}, Grid.IsEditing: {grid.IsEditing}");
				grid.CellEdited += (sender, e) => Log.Write(sender, $"CellEdited {CellInfo(e)}, Grid.IsEditing: {grid.IsEditing}");
				var customCell = new CustomCell();
				customCell.CreateCell = args =>
				{
					var textBox = new TextBox { ShowBorder = false, BackgroundColor = Colors.Transparent };

					if (!Platform.Instance.IsMac)
					{
						textBox.GotFocus += (sender, e) => textBox.BackgroundColor = SystemColors.ControlBackground;
						textBox.LostFocus += (sender, e) => textBox.BackgroundColor = Colors.Transparent;

						// ugly, there should be a better way to do this..
						var colorBinding = textBox.Bind(c => c.TextColor, args, Binding.Property((CellEventArgs a) => a.CellTextColor).Convert(c => args.IsEditing ? SystemColors.ControlText : c));
						args.PropertyChanged += (sender, e) =>
						{
							if (e.PropertyName == nameof(CellEventArgs.IsEditing))
								colorBinding.Update();
						};
					}
					else
					{
						// macOS handles colors more automaticcally for a TextBox
					}

					textBox.TextBinding.BindDataContext((GridTestItem i) => i.Text);

					return textBox;
				};
				AddLogging(customCell);
				grid.Columns.Add(new GridColumn { DataCell = customCell, Editable = true, HeaderText = "CustomTextBox" });

				var customCell2 = new CustomCell();
				customCell2.CreateCell = args =>
				{
					var dropDown = new DropDown { Items = { "Item 1", "Item 2", "Item 3" } };

					return dropDown;
				};
				AddLogging(customCell2);
				grid.Columns.Add(new GridColumn { DataCell = customCell2, Editable = true, HeaderText = "CustomDropDown" });

				var customCell3 = new CustomCell();
				customCell3.CreateCell = args =>
				{
					var checkBox = new CheckBox();

					return checkBox;
				};
				AddLogging(customCell3);
				grid.Columns.Add(new GridColumn { DataCell = customCell3, Editable = true, HeaderText = "CustomCheckBox" });

				grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell(nameof(GridTestItem.Text)), HeaderText = "TextBoxCell", Editable = true });

				var list = new TreeGridItemCollection();
				list.Add(new GridTestItem { Text = "Item 1" });
				list.Add(new GridTestItem { Text = "Item 2" });
				list.Add(new GridTestItem { Text = "Item 3" });
				SetDataStore(grid, list);

				// using MouseDown so the buttons don't get focus
				var beginEditButton = new Button { Text = "BeginEdit" };
				beginEditButton.MouseDown += (sender, e) =>
				{
					grid.BeginEdit(1, 0);
					e.Handled = true;
				};

				var commitEditButton = new Button { Text = "CommitEdit" };
				commitEditButton.MouseDown += (sender, e) =>
				{
					grid.CommitEdit();
					e.Handled = true;
				};

				var cancelEditButton = new Button { Text = "CancelEdit" };
				cancelEditButton.MouseDown += (sender, e) =>
				{
					grid.CancelEdit();
					e.Handled = true;
				};

				return new TableLayout(
					TableLayout.Horizontal(4, beginEditButton, commitEditButton, cancelEditButton, null),
					grid
					);
			});
		}

		public IEnumerable<object> CreateDataStore(int rows = 20)
		{
			var list = new TreeGridItemCollection();
			for (int i = 0; i < rows; i++)
			{
				list.Add(new GridTestItem { Text = $"Item {i}", Values = new[] { $"col {i}.2", $"col {i}.3", $"col {i}.4" } });
			}
			return list;
		}

		[ManualTest]
		[TestCase(0, -1)]
		[TestCase(1, -1)]
		[TestCase(2, -1)]
		[TestCase(0, 1)]
		[TestCase(1, 2)]
		[TestCase(0, 2)]
		public void ExpandedColumnShouldExpand(int columnToExpand, int secondColumn)
		{
			ManualForm("First Column should be expanded, and change size with the Grid", form =>
			{
				var grid = new T();
				SetDataStore(grid, CreateDataStore());

				grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell { Binding = Binding.Property((GridTestItem m) => m.Text) } });
				grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell(0) });
				grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell(1) });

				var expandColumn = grid.Columns[columnToExpand];
				expandColumn.HeaderText = "Expanded";
				expandColumn.Expand = true;

				if (secondColumn != -1)
				{
					var expandColumn2 = grid.Columns[secondColumn];
					expandColumn2.HeaderText = "Expanded2";
					expandColumn2.Expand = true;
				}

				return grid;
			});
		}

		[ManualTest]
		[TestCase(0, -1)]
		[TestCase(1, -1)]
		[TestCase(2, -1)]
		[TestCase(0, 1)]
		[TestCase(1, 2)]
		[TestCase(0, 2)]
		public void ExpandedColumnShouldAutoSize(int columnToExpand, int secondColumn)
		{
			ManualForm("First Column should be expanded, and change size with the Grid", form =>
			{
				var grid = new T();
				SetDataStore(grid, CreateDataStore());

				grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell { Binding = Binding.Property((GridTestItem m) => m.Text) } });
				grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell(0) });
				grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell(1) });

				var expandColumn = grid.Columns[columnToExpand];
				expandColumn.HeaderText = "Expanded";
				expandColumn.Expand = true;

				if (secondColumn != -1)
				{
					var expandColumn2 = grid.Columns[secondColumn];
					expandColumn2.HeaderText = "Expanded2";
					expandColumn2.Expand = true;
				}

				return TableLayout.AutoSized(grid);
			});
		}

		[Test, ManualTest]
		public void HeaderTextAlignmentShouldWork()
		{
			ManualForm("Check alignment for each header", form =>
			{
				var grid = new T();
				grid.Height = 200;
				SetDataStore(grid, CreateDataStore());

				grid.Columns.Add(new GridColumn
				{
					DataCell = new TextBoxCell { Binding = Binding.Property((GridTestItem m) => m.Text) },
					HeaderText = "Left Aligned",
					Width = 200
				});
				grid.Columns.Add(new GridColumn
				{
					DataCell = new TextBoxCell(0),
					HeaderText = "Also Left",
					HeaderTextAlignment = TextAlignment.Left,
					Width = 200
				});
				grid.Columns.Add(new GridColumn
				{
					DataCell = new TextBoxCell(1),
					HeaderText = "Center",
					HeaderTextAlignment = TextAlignment.Center,
					Width = 200
				});
				grid.Columns.Add(new GridColumn
				{
					DataCell = new TextBoxCell(2),
					HeaderText = "Right",
					HeaderTextAlignment = TextAlignment.Right,
					Width = 200
				});

				return grid;
			});
		}

		[Test, ManualTest]
		public void SettingWidthShouldDisableAutosize()
		{
			ManualForm("Width of column should be 300px and not change when scrolling",
			form =>
			{
				var control = new T();
				control.Width = 400;
				control.Height = 200;
				var column = new GridColumn
				{
					DataCell = new TextBoxCell(0),
					AutoSize = true,
					Width = 300, // setting width should set AutoSize to false
					HeaderText = "Cell"
				};
				control.Columns.Add(column);

				Assert.IsFalse(column.AutoSize, "#1");

				var dd = new TreeGridItemCollection();
				for (int i = 0; i < 1000; i++)
				{
					dd.Add(new TreeGridItem { Values = new[] { "Row " + i } });
				}
				SetDataStore(control, dd);

				return control;
			});
		}

		protected abstract void SetDataStore(T grid, IEnumerable<object> dataStore);


		[Test, ManualTest]
		public void ColumnWidthShouldNotIncludeSpacing() => ManualForm("Columns should be exactly 100 pixels wide", form =>
		{
			var grid = new T();
			grid.Width = 250;
			grid.Height = 200;
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell(0), Width = 100 });
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell(1), Width = 100 });

			var data = CreateDataStore(10);
			SetDataStore(grid, data);

			return grid;
		});

		[Test, ManualTest]
		public void CustomCellShouldGetMouseEvents()
		{
			int mouseMoved = 0;
			int mode = 0;
			Dialog(window =>
			{
				var cell = new CustomCell();
				cell.CreateCell = args =>
				{
					var label = new Label { VerticalAlignment = VerticalAlignment.Center };
					label.Text = "Click me";
					label.MouseMove += (sender, e) =>
					{
						Log.Write(sender, $"MouseMove {mouseMoved++}, {label.DataContext}");
					};
					label.MouseDown += (sender, e) =>
					{
						Log.Write(sender, $"MouseDown {label.DataContext}");
						if (mode == 0 && e.Buttons == MouseButtons.Primary)
						{
							label.Text = "Click again";
							e.Handled = true; // on some platforms (e.g. Mac) this can prevent selection
							mode = 1;
							return;
						}
						if (mode == 1 && e.Buttons == MouseButtons.Primary)
						{
							label.Text = "Right click me";
							mode = 2;
							return;
						}
						if (mode == 2 && e.Buttons == MouseButtons.Alternate)
						{
							label.Text = "Now double click me";
							mode = 3;
							return;
						}
					};
					label.MouseUp += (sender, e) => Log.Write(sender, "MouseUp");
					label.MouseDoubleClick += (sender, e) =>
					{
						Log.Write(sender, $"MouseDoubleClick {label.DataContext}");
						if (mode == 3)
						{
							mode = 4;
							window.Close();
							return;
						}
					};

					// A drawable should be able to get focus, then get all mouse events
					var drawable = new Drawable { Size = new Size(80, 16) };
					drawable.CanFocus = true;
					drawable.Paint += (sender, e) =>
					{
						e.Graphics.DrawText(SystemFonts.Default(), SystemColors.ControlText, 0, 0, "Drawable");
					};
					drawable.MouseDown += (sender, e) =>
					{
						Log.Write(sender, "MouseDown");
						// e.Handled = true;
					};
					drawable.MouseUp += (sender, e) =>
					{
						Log.Write(sender, "MouseUp");
					};
					drawable.MouseMove += (sender, e) =>
					{
						Log.Write(sender, "MouseMove");
					};
					drawable.GotFocus += (sender, e) => Log.Write(sender, "GotFocus");
					drawable.LostFocus += (sender, e) => Log.Write(sender, "LostFocus");

					return new TableLayout(new TableRow(label, new TextBox { Text = "A text box.." }, new CheckBox(), drawable, null));
				};

				var grid = new T();
				grid.Size = new Size(400, 400);
				grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell("Text") });
				grid.Columns.Add(new GridColumn { DataCell = cell, Expand = true, Editable = true });
				var list = new TreeGridItemCollection();
				list.Add(new GridTestItem { Text = "Item 1" });
				list.Add(new GridTestItem { Text = "Item 2" });
				list.Add(new GridTestItem { Text = "Item 3" });
				SetDataStore(grid, list);

				window.Content = grid;
			}, -1);
			Assert.NotZero(mouseMoved, "MouseMove was never fired!");
			Assert.AreEqual(4, mode, "Mode should be 4 after going through all steps");
		}

	}
}
