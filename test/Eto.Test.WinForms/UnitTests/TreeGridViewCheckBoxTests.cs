using Eto.Test.UnitTests;
using NUnit.Framework;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Eto.Test.WinForms.UnitTests
{
	[TestFixture]
	public class TreeGridViewCheckBoxTests : TestBase
	{
		const int WM_MOUSEMOVE = 0x0200;
		const int WM_LBUTTONDOWN = 0x0201;
		const int WM_LBUTTONUP = 0x0202;
		const int MK_LBUTTON = 0x0001;

		[DllImport("user32.dll")]
		static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		class MyItem : TreeGridItem
		{
			public bool? Checked { get; set; }
			public string Text { get; set; } = "";
		}

		static void Click(swf.Control control, int x, int y)
		{
			var lParam = (IntPtr)((y << 16) | (x & 0xFFFF));
			SendMessage(control.Handle, WM_MOUSEMOVE, IntPtr.Zero, lParam);
			SendMessage(control.Handle, WM_LBUTTONDOWN, (IntPtr)MK_LBUTTON, lParam);
			SendMessage(control.Handle, WM_LBUTTONUP, IntPtr.Zero, lParam);
		}

		[Test]
		public void CheckBoxInIndentedRowShouldToggleWhereItIsDrawn()
		{
			var child = new MyItem { Text = "Child" };
			TreeGridView tree = null;
			ShownAsync(form =>
			{
				tree = new TreeGridView { ShowHeader = false };
				tree.Columns.Add(new GridColumn
				{
					Editable = true,
					DataCell = new CheckBoxCell { Binding = Binding.Property((MyItem i) => i.Checked) }
				});
				tree.Columns.Add(new GridColumn
				{
					DataCell = new TextBoxCell { Binding = Binding.Property((MyItem i) => i.Text) }
				});

				var parent = new MyItem { Text = "Parent", Expanded = true };
				parent.Children.Add(child);
				tree.DataStore = new TreeGridItemCollection { parent };
				form.ClientSize = new Size(300, 200);
				form.Content = tree;
			}, async () =>
			{
				// let the grid lay out its rows before measuring them
				await Task.Delay(200);
				var grid = (swf.DataGridView)tree.ControlObject;
				var cell = grid.GetCellDisplayRectangle(0, 1, false);
				var y = cell.Y + cell.Height / 2;

				// find the checkbox by its pixels, as that's where the user would click
				int first = -1, last = -1;
				using (var bmp = new sd.Bitmap(grid.Width, grid.Height))
				{
					grid.DrawToBitmap(bmp, new sd.Rectangle(0, 0, grid.Width, grid.Height));
					var background = bmp.GetPixel(cell.X + 1, y);
					for (int x = cell.X + 1; x < cell.Right - 2; x++)
					{
						if (bmp.GetPixel(x, y) != background)
						{
							if (first < 0)
								first = x;
							last = x;
						}
					}
				}
				Assert.That(first, Is.GreaterThanOrEqualTo(0), "#1 checkbox should be drawn in the child row");
				var drawnCenter = (first + last) / 2;
				// where clicks used to land: the checkbox's spot without the tree's indent and expander
				var unindentedCenter = drawnCenter - grid.LogicalToDeviceUnits(32);
				Assert.That(unindentedCenter, Is.GreaterThan(cell.X), "#2 checkbox should be drawn indented");

				Click(grid, unindentedCenter, y);
				Assert.That(grid[0, 1].EditedFormattedValue, Is.EqualTo(false), "#3 clicking left of the checkbox should not toggle it");

				Click(grid, drawnCenter, y);
				Assert.That(grid[0, 1].EditedFormattedValue, Is.EqualTo(true), "#4 clicking the checkbox should toggle it");

				tree.CommitEdit();
				Assert.That(child.Checked, Is.True, "#5 toggled value should be pushed to the item");
			});
		}
	}
}
