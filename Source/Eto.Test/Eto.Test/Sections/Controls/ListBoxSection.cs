using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
using System.Collections;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(ListBox))]
	public class ListBoxSection : Scrollable
	{
		public ListBoxSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow(new Label { Text = "Default" }, Default());

			layout.AddRow(new Label { Text = "Virtual list, with Icons" }, WithIcons());

			if (Platform.Supports<ContextMenu>())
				layout.AddRow(new Label { Text = "Context Menu" }, WithContextMenu());

			layout.Add(null);

			Content = layout;
		}

		Control Default()
		{
			var control = new ListBox
			{
				Size = new Size(100, 150)
			};
			LogEvents(control);

			for (int i = 0; i < 10; i++)
			{
				control.Items.Add(new ListItem { Text = "Item " + i });
			}

			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };
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

		class VirtualList : IList, IEnumerable<object>
		{
			Icon image1 = TestIcons.TestIcon.WithSize(32, 32);
			Icon image2 = TestIcons.Logo.WithSize(16, 16);

			public bool UseVariableImages { get; set; }

			public int Count
			{
				get { return 1000; }
			}

			public object this[int index]
			{
				get
				{
					var item = new ImageListItem { Text = "Item " + index };
					if (UseVariableImages)
						item.Image = index % 2 == 0 ? image1 : image2;
					else
						item.Image = image1;
					return item;
				}
				set { }
			}

			public IEnumerator<object> GetEnumerator()
			{
				for (int i = 0; i < Count; i++)
					yield return this[i];
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public int Add(object value)
			{
				throw new System.NotImplementedException();
			}
			public void Clear()
			{
				throw new System.NotImplementedException();
			}
			public bool Contains(object value)
			{
				throw new System.NotImplementedException();
			}
			public int IndexOf(object value)
			{
				throw new System.NotImplementedException();
			}
			public void Insert(int index, object value)
			{
				throw new System.NotImplementedException();
			}
			public void Remove(object value)
			{
				throw new System.NotImplementedException();
			}
			public void RemoveAt(int index)
			{
				throw new System.NotImplementedException();
			}
			public bool IsFixedSize { get { return true; } }
			public bool IsReadOnly { get { return true; } }

			public void CopyTo(System.Array array, int index)
			{
				throw new System.NotImplementedException();
			}
			public bool IsSynchronized { get { return false; } }
			public object SyncRoot
			{
				get { throw new System.NotImplementedException(); }
			}
		}

		Control WithIcons()
		{
			var control = new ListBox
			{
				Size = new Size(100, 150)
			};
			LogEvents(control);

			control.DataStore = new VirtualList();

			var useVariableImages = new CheckBox { Text = "Use Variable Image Sizes" };
			useVariableImages.CheckedChanged += (sender, e) =>
			{
				control.DataStore = new VirtualList { UseVariableImages = useVariableImages.Checked ?? false };
			};

			return new StackLayout
			{
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				Items =
				{
					TableLayout.Horizontal(useVariableImages, null),
					control
				}
			};
		}

		Control WithContextMenu()
		{
			var control = new ListBox
			{
				Size = new Size(100, 150)
			};
			LogEvents(control);

			for (int i = 0; i < 10; i++)
			{
				control.Items.Add(new ListItem { Text = "Item " + i });
			}

			var menu = new ContextMenu();
			var item = new ButtonMenuItem { Text = "Click Me!" };
			item.Click += delegate
			{
				if (control.SelectedValue != null)
					Log.Write(item, "Click, Item: {0}", ((ListItem)control.SelectedValue).Text);
				else
					Log.Write(item, "Click, no item selected");
			};
			menu.Items.Add(item);

			control.ContextMenu = menu;
			return control;
		}

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

