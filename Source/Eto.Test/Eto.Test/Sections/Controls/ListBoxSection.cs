using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class ListBoxSection : Scrollable
	{
		public ListBoxSection()
		{
			var layout = new DynamicLayout();
			
			layout.AddRow(new Label { Text = "Default" }, Default());
						
			layout.AddRow(new Label { Text = "Virtual list, with Icons" }, WithIcons());

#if DESKTOP
			layout.AddRow(new Label { Text = "Context Menu" }, WithContextMenu());
#endif
			
			layout.Add(null);

			Content = layout;
		}

		Control Default()
		{
			var control = new ListBox
			{
				Size = new Size (100, 150)
			};
			LogEvents(control);

			for (int i = 0; i < 10; i++)
			{
				control.Items.Add(new ListItem { Text = "Item " + i });
			}
			
			var layout = new DynamicLayout();
			layout.Add(control);
			layout.BeginVertical();
			layout.AddRow(null, AddRowsButton(control), RemoveRowsButton(control), ClearButton(control), null);
			layout.EndVertical();
			
			return layout;
		}

		Control AddRowsButton(ListBox list)
		{
			var control = new Button { Text = "Add Rows" };
			control.Click += delegate
			{
				for (int i = 0; i < 10; i++)
					list.Items.Add(new ListItem { Text = "Item " + list.Items.Count });
			};
			return control;
		}

		Control RemoveRowsButton(ListBox list)
		{
			var control = new Button { Text = "Remove Rows" };
			control.Click += delegate
			{
				if (list.SelectedIndex >= 0)
					list.Items.RemoveAt(list.SelectedIndex);
			};
			return control;
		}

		Control ClearButton(ListBox list)
		{
			var control = new Button { Text = "Clear" };
			control.Click += delegate
			{
				list.Items.Clear();
			};
			return control;
		}

		class VirtualList : IListStore
		{
			Icon image = TestIcons.TestIcon;

			public int Count
			{
				get { return 1000; }
			}

			public IListItem this [int index]
			{
				get
				{
					return new ImageListItem { Text = "Item " + index, Image = image };
				}
			}
		}

		Control WithIcons()
		{
			var control = new ListBox
			{
				Size = new Size (100, 150)
			};
			LogEvents(control);
			
			control.DataStore = new VirtualList();
			return control;
		}
		#if DESKTOP
		Control WithContextMenu()
		{
			var control = new ListBox
			{
				Size = new Size (100, 150)
			};
			LogEvents(control);

			for (int i = 0; i < 10; i++)
			{
				control.Items.Add(new ListItem { Text = "Item " + i });
			}
			
			var menu = new ContextMenu();
			var item = new ImageMenuItem { Text = "Click Me!" };
			item.Clicked += delegate
			{
				if (control.SelectedValue != null)
					Log.Write(item, "Click, Item: {0}", control.SelectedValue.Text);
				else
					Log.Write(item, "Click, no item selected");
			};
			menu.MenuItems.Add(item);
			
			control.ContextMenu = menu;
			return control;
		}
		#endif
		void LogEvents(ListBox control)
		{
			control.SelectedIndexChanged += delegate
			{
				Log.Write(control, "SelectedIndexChanged, Index: {0}", control.SelectedIndex);
			};
			control.Activated += delegate
			{
				Log.Write(control, "Activated, Index: {0}", control.SelectedIndex);
			};
		}
	}
}

