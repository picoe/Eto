using Eto.Forms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class CustomCellTests : TestBase
	{
		class MyModel
		{
			public string Text { get; set; }
		}

		class MyCustomCell : StackLayout
		{
			public MyCustomCell()
			{
				Orientation = Orientation.Horizontal;
				Items.Add(new Label { Text = "Hello!" });
				var tb = new TextBox();
				tb.TextBinding.BindDataContext((MyModel m) => m.Text);
				Items.Add(new StackLayoutItem(tb, true));
			}
		}

		/// <summary>
		/// Test for WPF crash when a grid view and custom cell is on a separate tab and you switch to it.
		/// </summary>
		[Test]
		public void CustomCellOnSeparateTabLoadIssue()
		{
			TabControl tabs = null;
			Shown(form =>
			{
				var grid1 = new GridView { ShowHeader = false };
				grid1.Columns.Add(new GridColumn { DataCell = CustomCell.Create<MyCustomCell>() });
				grid1.DataStore = new List<MyModel>
				{
					new MyModel { Text = "Item 1"},
					new MyModel { Text = "Item 2"},
					new MyModel { Text = "Item 3"},
				};

				var grid2 = new GridView { ShowHeader = false };
				grid2.Columns.Add(new GridColumn { DataCell = CustomCell.Create<MyCustomCell>() });
				grid2.DataStore = new List<MyModel>
				{
					new MyModel { Text = "Item 1"},
					new MyModel { Text = "Item 2"},
					new MyModel { Text = "Item 3"},
				};

				tabs = new TabControl();
				tabs.Pages.Add(new TabPage(grid1) { Text = "Tab 1" });
				tabs.Pages.Add(new TabPage(grid2) { Text = "Tab 2" });
				form.Content = tabs;
			}, () =>
			{
				tabs.SelectedIndex = 1;
			});
		}
	}
}
