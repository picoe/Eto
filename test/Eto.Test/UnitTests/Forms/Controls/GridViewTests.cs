using NUnit.Framework;
using System.Runtime.ExceptionServices;
namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class GridViewTests : GridTests<GridView>
	{
		protected override void SetDataStore(GridView grid, IEnumerable<object> dataStore)
		{
			grid.DataStore = dataStore;
		}

		[Test, ManualTest]
		public void MultipleChangesShouldWork() => ManualForm(
			"Scroll while the collection is updated,\nand ensure all items are correct and not duplicated.",
			form =>
			{

				var collection = new ObservableCollection<GridItem>();
				for (int i = 0; i < 20; i++)
				{
					collection.Add(new GridItem(true, $"Item {i}"));
				}

				var gv = new GridView
				{
					DataStore = collection,
					Size = new Size(200, 200),
					Columns = {
						new GridColumn { HeaderText = "Check", DataCell = new CheckBoxCell(0) },
						new GridColumn { HeaderText = "Text", DataCell = new TextBoxCell(1) }
					},
				};

				Application.Instance.AsyncInvoke(async () =>
				{
					for (int i = 0; i < collection.Count; i++)
					{
						if (!form.Loaded)
							return;
						// gv.SelectedRow = i;
						await Task.Delay(1000);
						collection[i] = new GridItem(true, $"Changed {i}");
					}
				});
				return gv;
			});

		[Test, ManualTest]
		public void CellClickShouldHaveMouseInformation()
		{
			Exception exception = null;
			Form(form =>
			{
				var label = new Label { Text = "Left click on the cell at 0, 0" };
				var gv = new GridView { Size = new Size(200, 100) };

				gv.Columns.Add(new GridColumn
				{
					DataCell = new CheckBoxCell(0),
					HeaderText = "Check"
				});
				gv.Columns.Add(new GridColumn
				{
					DataCell = new TextBoxCell(1),
					HeaderText = "Text"
				});
				gv.DataStore = new List<GridItem>
				{
					new GridItem(true, "Item 1"),
					new GridItem(false, "Item 2"),
					new GridItem(false, "Item 3")
				};

				var step = 0;


				gv.CellClick += (sender, e) =>
				{
					try
					{
						switch (step)
						{
							case 0:
								Assert.That(e.Column, Is.EqualTo(0));
								Assert.That(e.Row, Is.EqualTo(0));
								Assert.That(e.Buttons, Is.EqualTo(MouseButtons.Primary));
								Assert.That(e.Modifiers, Is.EqualTo(Keys.None));
								Assert.That(Point.Round(gv.PointToScreen(e.Location) / 4f), Is.EqualTo(Point.Round(Mouse.Position / 4f)));
								label.Text = "Now, left click on 1, 0";
								step = 1;
								break;
							case 1:
								Assert.That(e.Column, Is.EqualTo(1));
								Assert.That(e.Row, Is.EqualTo(0));
								Assert.That(e.Buttons, Is.EqualTo(MouseButtons.Primary));
								Assert.That(e.Modifiers, Is.EqualTo(Keys.None));
								Assert.That(gv.PointToScreen(e.Location), Is.EqualTo(Mouse.Position));
								label.Text = "Now, right click on 1, 1";
								step = 2;
								break;
							case 2:
								Assert.That(e.Column, Is.EqualTo(1));
								Assert.That(e.Row, Is.EqualTo(1));
								Assert.That(e.Buttons, Is.EqualTo(MouseButtons.Alternate));
								Assert.That(e.Modifiers, Is.EqualTo(Keys.None));
								Assert.That(gv.PointToScreen(e.Location), Is.EqualTo(Mouse.Position));
								label.Text = "Now, right click on 1, 2 with the shift key pressed";
								step = 3;
								break;
							case 3:
								Assert.That(e.Column, Is.EqualTo(1));
								Assert.That(e.Row, Is.EqualTo(2));
								Assert.That(e.Buttons, Is.EqualTo(MouseButtons.Alternate));
								Assert.That(e.Modifiers, Is.EqualTo(Keys.Shift));
								Assert.That(gv.PointToScreen(e.Location), Is.EqualTo(Mouse.Position));
								step = 4;
								form.Close();
								break;
							default:
								Assert.Fail("Test is in an invalid state");
								break;
						}
					}
					catch (Exception ex)
					{
						exception = ex;
						form.Close();
					}
				};

				form.Content = new StackLayout
				{
					HorizontalContentAlignment = HorizontalAlignment.Stretch,
					Items = {
						label,
						gv
					}
				};
			}, -1);
			if (exception != null)
				ExceptionDispatchInfo.Capture(exception).Throw();
		}

		class MyCollection : ObservableCollection<DataItem>
		{
			public void AddRange(IEnumerable<DataItem> items)
			{
				foreach (var item in items)
					Items.Add(item);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		[Test, ManualTest]
		public void CollectionChangedWithResetShouldShowItems()
		{
			var count = 10;
			ManualForm($"GridView should show {count} items", form =>
			{
				var collection = new MyCollection();
				var filterCollection = new FilterCollection<DataItem>(collection);
				var myGridView = new GridView
				{
					Size = new Size(200, 260),
					DataStore = filterCollection,
					Columns = {
						new GridColumn {
							DataCell = new TextBoxCell { Binding = Eto.Forms.Binding.Property((DataItem m) => m.Id.ToString()) }
						}
					}
				};
				collection.Clear();
				collection.AddRange(Enumerable.Range(1, count).Select(r => new DataItem(r)));

				return myGridView;
			});

		}



		class CustomCellWithTableLayout : CustomCell
		{
			protected override Control OnCreateCell(CellEventArgs args)
			{
				var label = new Label { Text = "Hello" };

				var button = new Button { MinimumSize = Size.Empty, Text = "..." };
				button.Bind(c => c.Visible, args, a => a.IsSelected); // kaboom when reloading!

				return new TableLayout
				{
					Rows = { new TableRow(new TableCell(label, true), button) }
				};
			}
		}

		[Test]
		public void ReloadingDataStoreShouldNotCrash()
		{
			Form f = null;
			GridView g = null;
			try
			{
				Application.Instance.Invoke(() =>
				{
					f = new Form { Size = new Size(300, 300) };

					g = new GridView();
					g.Columns.Add(new GridColumn
					{
						DataCell = new CustomCellWithTableLayout()
					});
					g.DataStore = Enumerable.Range(0, 100).Cast<object>().ToList();
					g.SelectedRow = 1;
					f.Content = g;
					f.Show();
				});

				// give the grid time to render its custom cells (where the crash used to happen) before
				// and after the reload. There's nothing observable to wait on, so this stays a fixed
				// sleep - just a short one, as a few frames is all the render pass needs.
				Thread.Sleep(250);

				Application.Instance.Invoke(() =>
				{
					g.DataStore = Enumerable.Range(0, 10).Cast<object>().ToList();
				});

				Thread.Sleep(250);
			}
			finally
			{
				Application.Instance.Invoke(() => f?.Close());
			}
		}

		[TestCase(true, true)]
		[TestCase(true, false)]
		[TestCase(false, true)]
		[TestCase(false, false)]
		public void ClickingWithEmptyDataShouldNotCrash(bool allowEmptySelection, bool allowMultipleSelection)
		{
			Exception exception = null;
			Form(form =>
			{
				var dd = new List<GridItem>();

				dd.Add(new GridItem { Values = new[] { "Hello" } });
				var control = new GridView();
				control.AllowEmptySelection = allowEmptySelection;
				control.AllowMultipleSelection = allowMultipleSelection;
				control.Columns.Add(new GridColumn
				{
					DataCell = new TextBoxCell(0),
					Width = 100,
					HeaderText = "Text Cell"
				});
				control.DataStore = dd;
				Application.Instance.AsyncInvoke(() =>
				{
					// can crash when had selection initially but no selection after.
					try
					{
						control.DataStore = new List<GridItem>();
					}
					catch (Exception ex)
					{
						exception = ex;
					}
					Application.Instance.AsyncInvoke(form.Close);
				});

				form.Content = control;
			});

			if (exception != null)
				ExceptionDispatchInfo.Capture(exception).Throw();
		}

		[Test]
		public void UsingStringListAsDataStoreShouldNotEmitCriticalWarnings()
		{
			var output = StandardErrorCapture.Capture(() =>
			{
				ShownAsync(form =>
				{
					var grid = new GridView { Size = new Size(200, 200) };
					grid.Columns.Add(new GridColumn
					{
						HeaderText = "Text",
						DataCell = new TextBoxCell { Binding = Binding.Property((string s) => s) }
					});
					grid.DataStore = new List<string> { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5" };
					return grid;
				}, async grid =>
				{
					// let the grid render its cells - that's when GTK emits the critical if the data store is
					// unsupported. Nothing to poll on (we're waiting for output that should never arrive),
					// so this is a fixed delay, kept to a handful of frames.
					await Task.Delay(200);
					grid.ReloadData(Enumerable.Range(0, 5));
					await Task.Delay(200);
				});
			});

			Assert.That(output, Does.Not.Contain("CRITICAL"), $"A critical warning was emitted while rendering a List<string> data store:{Environment.NewLine}{output}");
		}

		[TestCase(WindowStyle.Default, false)]
		[TestCase(WindowStyle.None, true)]
		public void PreferredSizeShouldAccountForAllRowsWhenLargerThanWindow(WindowStyle windowStyle, bool positionRelativeToParent) => Async(10000, async () =>
		{
			const int rowHeight = 20;
			const int rowCount = 5;
			const int largerRowCount = 10;

			static ObservableCollection<DataItem> CreateRows(int rows)
			{
				var collection = new ObservableCollection<DataItem>();
				for (int i = 0; i < rows; i++)
					collection.Add(new DataItem(i) { TextValue = $"Item {i}" });
				return collection;
			}

			var grid = new GridView { ShowHeader = false, RowHeight = rowHeight };
			grid.Columns.Add(new GridColumn
			{
				DataCell = new TextBoxCell { Binding = Binding.Property((DataItem m) => m.TextValue) }
			});
			grid.DataStore = CreateRows(rowCount);

			Window parent = Application.Instance.MainForm;
			bool closeParent = false;
			var form = new FloatingForm
			{
				WindowStyle = windowStyle,
				Content = grid,
				ShowInTaskbar = false,
				ShowActivated = false,
				CanFocus = false
			};
			// size the child form to fit its content
			form.Size = Size.Ceiling(grid.GetPreferredSize());
			try
			{
				if (positionRelativeToParent)
				{
					if (parent == null)
					{
						// a WindowStyle.None form (e.g. a popup) is typically owned by and
						// positioned relative to a parent window
						var parentForm = new Form { Content = new Panel(), ClientSize = new Size(400, 400), Location = new Point(100, 100) };
						closeParent = true;
						var parentShown = WaitEventAsync<EventArgs>(h => parentForm.Shown += h);
						parentForm.Show();
						parent = parentForm;
						await parentShown;
					}
					form.Owner = parent;
					form.Location = parent.Location + new Size(20, 20);
				}

				var shown = WaitEventAsync<EventArgs>(h => form.Shown += h);
				form.Show();
				await shown;
				await WaitUntil(() => grid.GetPreferredSize().Height >= rowCount * rowHeight, 1000);

				var preferredSize = grid.GetPreferredSize();

				// The preferred size should reflect the content needed to display every row.
				Assert.That(preferredSize.Height, Is.GreaterThanOrEqualTo(rowCount * rowHeight), "#1 Preferred height should account for all rows");

				// Hide the same window, swap in a larger list, re-size the child form to the new
				// content, then re-show it and ensure the preferred size grows to account for the
				// additional rows.
				form.Visible = false;
				await WaitUntil(() => !form.Visible, 100);
				grid.DataStore = CreateRows(largerRowCount);
				form.Size = Size.Ceiling(grid.GetPreferredSize());
				form.Visible = true;
				await WaitUntil(() => grid.GetPreferredSize().Height >= largerRowCount * rowHeight, 1000);

				var largerSize = grid.GetPreferredSize();
				Assert.That(largerSize.Height, Is.GreaterThanOrEqualTo(largerRowCount * rowHeight), "#2 Preferred height should account for all rows in the larger list");
				Assert.That(largerSize.Height, Is.GreaterThan(preferredSize.Height), "#3 Preferred height should be larger when re-shown with more rows");
			}
			finally
			{
				// Wait for the windows to actually be gone before returning. Closing is asynchronous, so
				// without this the next test's window can come up while these are still on screen and
				// never becomes active - which breaks tests that need their control to take focus.
				await CloseAsync(form);
				if (closeParent)
					await CloseAsync(parent);
			}
		});
	}
}
