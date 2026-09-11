using Eto.Drawing;
using Eto.Forms;
using Eto.Test.UnitTests;
using NUnit.Framework;
using System.Windows.Threading;

namespace Eto.Test.Wpf.UnitTests;

/// <summary>
/// Scrolling an item of a list or grid into view must scroll only that control, not the scrollable
/// containers it lives in. WPF re-raises the bring-into-view request from a ScrollViewer's
/// ScrollContentPresenter after it scrolls, so without suppressing that every scrollable ancestor
/// scrolls too and the whole panel or dialog jumps to the list.
/// </summary>
[TestFixture]
public class ScrollIntoViewTests : TestBase
{
	const int SelectedRow = 80;
	const int RowCount = 100;

	static Scrollable CreateScrollable(Control content) => new Scrollable
	{
		Content = new StackLayout
		{
			// tall content above and below so the scrollable can scroll in both directions
			Items =
			{
				new Panel { Size = new Size(200, 400) },
				content,
				new Panel { Size = new Size(200, 400) }
			}
		}
	};

	static ListBox CreateListBox()
	{
		var listBox = new ListBox { Height = 100 };
		for (int i = 0; i < RowCount; i++)
			listBox.Items.Add($"Item {i}");
		return listBox;
	}

	static GridView CreateGridView()
	{
		var items = new List<GridItem>();
		for (int i = 0; i < RowCount; i++)
			items.Add(new GridItem($"Item {i}"));

		return new GridView
		{
			Height = 100,
			DataStore = items,
			Columns = { new GridColumn { HeaderText = "Text", DataCell = new TextBoxCell(0) } }
		};
	}

	static double GetVerticalOffset(Control control)
	{
		var element = control.ControlObject as sw.FrameworkElement;
		element?.UpdateLayout();
		return FindChild<swc.ScrollViewer>(element)?.VerticalOffset ?? -1;
	}

	static T FindChild<T>(sw.DependencyObject parent)
		where T : sw.DependencyObject
	{
		if (parent == null)
			return null;
		var count = sw.Media.VisualTreeHelper.GetChildrenCount(parent);
		for (int i = 0; i < count; i++)
		{
			var child = sw.Media.VisualTreeHelper.GetChild(parent, i);
			var found = child as T ?? FindChild<T>(child);
			if (found != null)
				return found;
		}
		return null;
	}

	// RH-95952: the Sun dialog selects the nearest city in its list while the dialog is being built,
	// which scrolled the dialog's scrollable down to the list instead of just scrolling the list.
	[Test]
	public void SelectingListBoxItemShouldNotScrollParent()
	{
		ListBox listBox = null;
		ShownAsync<Scrollable>(form =>
		{
			form.ClientSize = new Size(300, 200);
			listBox = CreateListBox();
			// selected before the form is shown, as the Sun dialog does
			listBox.SelectedIndex = SelectedRow;
			var scrollable = CreateScrollable(listBox);
			form.Content = scrollable;
			return scrollable;
		}, async scrollable =>
		{
			// ItemsControl.ScrollIntoView defers to the dispatcher when the item containers have not
			// been generated yet, so give it a chance to run.
			await Task.Delay(200);

			Assert.That(GetVerticalOffset(listBox), Is.GreaterThan(0), "#1 list box should scroll to its selected item");
			Assert.That(scrollable.ScrollPosition.Y, Is.EqualTo(0), "#2 parent scrollable should not scroll");
		});
	}

	// A GridView brings its current cell into view when it is first laid out, which scrolls the
	// parent to show the grid, so scroll back to the top before testing ScrollToRow.
	[Test]
	public void ScrollingGridToRowShouldNotScrollParent()
	{
		GridView grid = null;
		ShownAsync<Scrollable>(form =>
		{
			form.ClientSize = new Size(300, 200);
			grid = CreateGridView();
			var scrollable = CreateScrollable(grid);
			form.Content = scrollable;
			return scrollable;
		}, async scrollable =>
		{
			await Task.Delay(200);
			scrollable.ScrollPosition = new Point(0, 0);
			await Task.Delay(200);
			Assume.That(scrollable.ScrollPosition.Y, Is.EqualTo(0), "parent scrollable should start at the top");

			grid.ScrollToRow(SelectedRow);
			await Task.Delay(200);

			Assert.That(GetVerticalOffset(grid), Is.GreaterThan(0), "#1 grid should scroll to the row");
			Assert.That(scrollable.ScrollPosition.Y, Is.EqualTo(0), "#2 parent scrollable should not scroll");
		});
	}

	// The handler removal is queued on the dispatcher, so it can run after the control that queued it
	// is long gone. Nothing should be left lingering, and nothing should throw.
	[Test]
	public void DisposingRightAfterSelectingShouldNotThrow()
	{
		Exception unhandled = null;
		DispatcherUnhandledExceptionEventHandler onUnhandled = (s, e) =>
		{
			unhandled = e.Exception;
			e.Handled = true;
		};
		Invoke(() =>
		{
			var dispatcher = Dispatcher.CurrentDispatcher;
			dispatcher.UnhandledException += onUnhandled;
			try
			{
				var form = new Form { ClientSize = new Size(300, 200) };
				var listBox = CreateListBox();
				form.Content = CreateScrollable(listBox);
				form.Show();

				listBox.SelectedIndex = SelectedRow;

				// destroyed before the queued removal has had a chance to run
				form.Close();
				form.Dispose();

				// pump past Background priority, where the removal was queued
				dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
			}
			finally
			{
				dispatcher.UnhandledException -= onUnhandled;
			}
		});

		Assert.That(unhandled, Is.Null, $"#1 removing the suppression handler after dispose should not throw ({unhandled})");
	}
}
