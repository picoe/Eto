using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Automation.Peers;
using Eto.Drawing;
using Eto.Forms;
using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Wpf.UnitTests;

/// <summary>
/// Right-clicking a row makes WPF's DataGrid focus the clicked cell before selecting its row. If
/// that focus commits an edit and the app reloads the grid in response, the row being selected
/// has already been thrown away by the time it gets selected.
/// </summary>
[TestFixture]
public class GridContextMenuTests : TestBase
{
	// UIA_SelectionItem_ElementAddedToSelectionEventId .. UIA_Selection_InvalidatedEventId
	static readonly int[] SelectionEventIds = { 20010, 20011, 20012, 20013 };

	static readonly Type EventMapType = typeof(AutomationPeer).Assembly.GetTypes().FirstOrDefault(r => r.Name == "EventMap");

	// Pretends a UI Automation client (e.g. a screen reader or touch keyboard) is listening for
	// selection events, which is when DataGrid builds peers for the selected items.
	static void SetSelectionListeners(bool listening)
	{
		var method = EventMapType?.GetMethod(listening ? "AddEvent" : "RemoveEvent", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
		Assume.That(method, Is.Not.Null, "Could not find EventMap to simulate an automation client");
		foreach (var id in SelectionEventIds)
			method.Invoke(null, new object[] { id });
	}

	static IEnumerable<T> FindChildren<T>(sw.DependencyObject parent)
		where T : sw.DependencyObject
	{
		var count = sw.Media.VisualTreeHelper.GetChildrenCount(parent);
		for (int i = 0; i < count; i++)
		{
			var child = sw.Media.VisualTreeHelper.GetChild(parent, i);
			if (child is T found)
				yield return found;
			foreach (var descendant in FindChildren<T>(child))
				yield return descendant;
		}
	}

	static void AddItems(ObservableCollection<GridItem> items)
	{
		for (int i = 0; i < 10; i++)
			items.Add(new GridItem($"Item {i}"));
	}

	static void RaiseContextMenuOpening(sw.UIElement element)
	{
		var args = (sw.RoutedEventArgs)Activator.CreateInstance(
			typeof(swc.ContextMenuEventArgs),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[] { element, true, -1.0, -1.0 },
			null);
		args.RoutedEvent = sw.FrameworkElement.ContextMenuOpeningEvent;
		element.RaiseEvent(args);
	}

	// RH-96988: right-clicking another row of the Layers or Linetypes panel while renaming crashed
	// Rhino, as committing the name reloads the panel's grid.
	[Test]
	public void RightClickingAnotherRowWhileEditingShouldNotThrow()
	{
		Invoke(() =>
		{
			var items = new ObservableCollection<GridItem>();
			AddItems(items);
			var grid = new GridView
			{
				Size = new Size(200, 300),
				DataStore = items,
				ContextMenu = new ContextMenu { Items = { new ButtonMenuItem { Text = "Item" } } },
				Columns = { new GridColumn { HeaderText = "Text", DataCell = new TextBoxCell(0), Editable = true } }
			};
			grid.CellEdited += (sender, e) =>
			{
				// reload the rows as the Rhino panels do when their table changes
				items.Clear();
				AddItems(items);
			};
			var form = new Form { Content = grid };
			try
			{
				form.Show();
				var dataGrid = (swc.DataGrid)grid.ControlObject;
				dataGrid.UpdateLayout();
				UIElementAutomationPeer.CreatePeerForElement(dataGrid);

				grid.SelectRow(0);
				grid.BeginEdit(0, 0);
				dataGrid.UpdateLayout();
				Assume.That(grid.IsEditing, Is.True, "grid should be editing");

				var otherCell = FindChildren<swc.DataGridCell>(dataGrid).First(r => r.DataContext == items[3]);

				SetSelectionListeners(true);
				try
				{
					Assert.DoesNotThrow(() => RaiseContextMenuOpening(otherCell), "#1 right-clicking another row while editing should not throw");
				}
				finally
				{
					SetSelectionListeners(false);
				}

				Assert.That(grid.SelectedItems.Cast<object>(), Has.None.Null, "#2 selection should not contain a discarded row");
			}
			finally
			{
				form.Close();
			}
		});
	}

	[TestCase(true)]
	[TestCase(false)]
	public void RightClickingAnotherRowShouldSelectIt(bool editing)
	{
		Invoke(() =>
		{
			var items = new ObservableCollection<GridItem>();
			AddItems(items);
			var grid = new GridView
			{
				Size = new Size(200, 300),
				DataStore = items,
				ContextMenu = new ContextMenu { Items = { new ButtonMenuItem { Text = "Item" } } },
				Columns = { new GridColumn { HeaderText = "Text", DataCell = new TextBoxCell(0), Editable = true } }
			};
			var form = new Form { Content = grid };
			try
			{
				form.Show();
				var dataGrid = (swc.DataGrid)grid.ControlObject;
				grid.SelectRow(0);
				if (editing)
					grid.BeginEdit(0, 0);
				dataGrid.UpdateLayout();

				var otherCell = FindChildren<swc.DataGridCell>(dataGrid).First(r => r.DataContext == items[3]);
				RaiseContextMenuOpening(otherCell);

				Assert.That(grid.SelectedRows, Is.EqualTo(new[] { 3 }), "#1 right-clicked row should be selected");
				Assert.That(grid.IsEditing, Is.False, "#2 edit should be committed");
			}
			finally
			{
				form.Close();
			}
		});
	}
}
