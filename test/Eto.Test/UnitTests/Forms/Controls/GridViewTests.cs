using System;
using NUnit.Framework;
using Eto.Forms;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Eto.Drawing;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class GridViewTests : TestBase
	{
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
								Assert.AreEqual(0, e.Column);
								Assert.AreEqual(0, e.Row);
								Assert.AreEqual(MouseButtons.Primary, e.Buttons);
								Assert.AreEqual(Keys.None, e.Modifiers);
								Assert.AreEqual(Point.Round(Mouse.Position / 4f), Point.Round(gv.PointToScreen(e.Location) / 4f));
								label.Text = "Now, left click on 1, 0";
								step = 1;
								break;
							case 1:
								Assert.AreEqual(1, e.Column);
								Assert.AreEqual(0, e.Row);
								Assert.AreEqual(MouseButtons.Primary, e.Buttons);
								Assert.AreEqual(Keys.None, e.Modifiers);
								Assert.AreEqual(Mouse.Position, gv.PointToScreen(e.Location));
								label.Text = "Now, right click on 1, 1";
								step = 2;
								break;
							case 2:
								Assert.AreEqual(1, e.Column);
								Assert.AreEqual(1, e.Row);
								Assert.AreEqual(MouseButtons.Alternate, e.Buttons);
								Assert.AreEqual(Keys.None, e.Modifiers);
								Assert.AreEqual(Mouse.Position, gv.PointToScreen(e.Location));
								label.Text = "Now, right click on 1, 2 with the shift key pressed";
								step = 3;
								break;
							case 3:
								Assert.AreEqual(1, e.Column);
								Assert.AreEqual(2, e.Row);
								Assert.AreEqual(MouseButtons.Alternate, e.Buttons);
								Assert.AreEqual(Keys.Shift, e.Modifiers);
								Assert.AreEqual(Mouse.Position, gv.PointToScreen(e.Location));
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

		[Test, ManualTest]
		public void AutoSizedColumnShouldChangeSizeOfControl()
		{
			ManualForm("GridView should auto size to content", form =>
			{
				var collection = new ObservableCollection<DataItem>();
				var gridView = new GridView
				{
					Height = 180,
					DataStore = collection,
					Columns =
					{
						new GridColumn
						{
							AutoSize = true,
							DataCell = new TextBoxCell { Binding = Binding.Property((DataItem m) => m.TextValue) }
						}
					}
				};
				var item = new DataItem { TextValue = "Some Text" };
				collection.Add(item);

				var textBox = new TextBox();
				textBox.Focus();
				textBox.TextBinding.Bind(item, i => i.TextValue);
				textBox.TextChanged += (sender, e) => gridView.ReloadData(0);

				var layout = new DynamicLayout();
				layout.BeginVertical(yscale: true);
				layout.AddRow(gridView, null); // gridView is auto sized
				layout.EndVertical();

				layout.AddSeparateRow("Text:", textBox);

				return layout;
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

				Thread.Sleep(1000);

				Application.Instance.Invoke(() =>
				{
					g.DataStore = Enumerable.Range(0, 10).Cast<object>().ToList();
				});

				Thread.Sleep(1000);
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
				Application.Instance.AsyncInvoke(() => {
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
	}
}
