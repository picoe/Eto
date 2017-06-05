using System;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;

namespace Eto.Test.Sections.Controls
{
	[Obsolete("Since 2.4")]
	[Section("Controls", typeof(TreeView))]
	public class TreeViewSection : Scrollable
	{
		int expanded;
		readonly CheckBox allowCollapsing;
		readonly CheckBox allowExpanding;
		readonly TreeView treeView;
		int newItemCount;
		static readonly Image Image = TestIcons.TestIcon;
		Label hoverNodeLabel;
		bool cancelLabelEdit;

		public TreeViewSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.BeginHorizontal();
			layout.Add(new Label());
			layout.BeginVertical();
			layout.BeginHorizontal();
			layout.Add(null);
			layout.Add(allowExpanding = new CheckBox { Text = "Allow Expanding", Checked = true });
			layout.Add(allowCollapsing = new CheckBox { Text = "Allow Collapsing", Checked = true });
			layout.Add(RefreshButton());
			layout.Add(null);
			layout.EndHorizontal();
			layout.EndVertical();
			layout.EndHorizontal();

			treeView = ImagesAndMenu();

			layout.AddRow(new Label { Text = "Simple" }, Default());
			layout.BeginHorizontal();
			layout.Add(new Panel());
			layout.BeginVertical();
			layout.AddSeparateRow(InsertButton(), AddChildButton(), RemoveButton(), ExpandButton(), CollapseButton(), null);
			layout.AddSeparateRow(LabelEditCheck(), EnabledCheck(), null);
			layout.EndVertical();
			layout.EndHorizontal();
			layout.AddRow(new Label { Text = "With Images\n&& Context Menu" }, treeView);
			layout.AddRow(new Panel(), HoverNodeLabel());

			layout.Add(null, false, true);

			Content = layout;
		}

		Control HoverNodeLabel()
		{
			hoverNodeLabel = new Label();

			treeView.MouseMove += (sender, e) =>
			{
				var node = treeView.GetNodeAt(e.Location);
				hoverNodeLabel.Text = "Item under mouse: " + (node != null ? node.Text : "(no node)");
			};

			return hoverNodeLabel;
		}

		Control InsertButton()
		{
			var control = new Button { Text = "Insert" };
			control.Click += (sender, e) =>
			{
				var item = treeView.SelectedItem as TreeItem;
				var parent = (item != null ? item.Parent : treeView.DataStore) as TreeItem;
				if (parent != null)
				{
					var index = item != null ? parent.Children.IndexOf(item) : 0;
					parent.Children.Insert(index, new TreeItem { Text = "New Item " + newItemCount++ });
					if (item != null)
						treeView.RefreshItem(parent);
					else
						treeView.RefreshData();
				}
			};
			return control;
		}

		Control AddChildButton()
		{
			var control = new Button { Text = "Add Child" };
			control.Click += (sender, e) =>
			{
				var item = treeView.SelectedItem as TreeItem;
				if (item != null)
				{
					item.Children.Add(new TreeItem { Text = "New Item " + newItemCount++ });
					treeView.RefreshItem(item);
				}
			};
			return control;
		}

		Control RemoveButton()
		{
			var control = new Button { Text = "Remove" };
			control.Click += (sender, e) =>
			{
				var item = treeView.SelectedItem as TreeItem;
				if (item != null)
				{
					var parent = item.Parent as TreeItem;
					parent.Children.Remove(item);
					if (parent.Parent == null)
						treeView.RefreshData();
					else
						treeView.RefreshItem(parent);
				}
			};
			return control;
		}

		Control RefreshButton()
		{
			var control = new Button { Text = "Refresh" };
			control.Click += (sender, e) =>
			{
				foreach (var tree in Children.OfType<TreeView>())
				{
					tree.RefreshData();
				}
			};
			return control;
		}

		Control ExpandButton()
		{
			var control = new Button { Text = "Expand" };
			control.Click += (sender, e) =>
			{
				var item = treeView.SelectedItem;
				if (item != null)
				{
					item.Expanded = true;
					treeView.RefreshItem(item);
				}
			};
			return control;
		}

		Control CollapseButton()
		{
			var control = new Button { Text = "Collapse" };
			control.Click += (sender, e) =>
			{
				var item = treeView.SelectedItem;
				if (item != null)
				{
					item.Expanded = false;
					treeView.RefreshItem(item);
				}
			};
			return control;
		}

		Control CancelLabelEdit()
		{
			var control = new CheckBox { Text = "Cancel Edit" };
			control.CheckedChanged += (sender, e) => cancelLabelEdit = control.Checked ?? false;
			return control;
		}

		Control LabelEditCheck()
		{
			var control = new CheckBox { Text = "LabelEdit", Checked = treeView.LabelEdit };
			control.CheckedChanged += (sender, e) => treeView.LabelEdit = control.Checked ?? false;
			return control;
		}

		Control EnabledCheck()
		{
			var control = new CheckBox { Text = "Enabled", Checked = treeView.Enabled };
			control.CheckedChanged += (sender, e) => treeView.Enabled = control.Checked ?? false;
			return control;
		}

		TreeItem CreateTreeItem(int level, string name, Image image)
		{
			var item = new TreeItem
			{
				Text = name,
				Expanded = expanded++ % 2 == 0,
				Image = image
			};
			if (level < 4)
			{
				for (int i = 0; i < 4; i++)
				{
					item.Children.Add(CreateTreeItem(level + 1, name + " " + i, image));
				}
			}
			return item;
		}

		Control Default()
		{
			var control = new TreeView
			{
				Size = new Size(100, 150)
			};
			control.DataStore = CreateTreeItem(0, "Item", null);
			LogEvents(control);
			return control;
		}

		TreeView ImagesAndMenu()
		{
			var control = new TreeView
			{
				Size = new Size(100, 150)
			};

			if (Platform.Supports<ContextMenu>())
			{
				var menu = new ContextMenu();
				var item = new ButtonMenuItem { Text = "Click Me!" };
				item.Click += delegate
				{
					if (control.SelectedItem != null)
						Log.Write(item, "Click, Rows: {0}", control.SelectedItem.Text);
					else
						Log.Write(item, "Click, no item selected");
				};
				menu.Items.Add(item);

				control.ContextMenu = menu;
			}

			control.DataStore = CreateTreeItem(0, "Item", Image);
			LogEvents(control);
			return control;
		}

		void LogEvents(TreeView control)
		{
			control.LabelEditing += (sender, e) =>
			{
				if (cancelLabelEdit)
				{
					Log.Write(control, "BeforeLabelEdit (cancelled), Item: {0}", e.Item.Text);
					e.Cancel = true;
				}
				else
					Log.Write(control, "BeforeLabelEdit, Item: {0}", e.Item.Text);
			};
			control.LabelEdited += (sender, e) =>
			{
				Log.Write(control, "AfterLabelEdit, Item: {0}, New Label: {1}", e.Item.Text, e.Label);
			};
			control.Activated += delegate(object sender, TreeViewItemEventArgs e)
			{
				Log.Write(control, "Activated, Item: {0}", e.Item.Text);
			};
			control.SelectionChanged += delegate
			{
				Log.Write(control, "SelectionChanged, Item: {0}", control.SelectedItem != null ? control.SelectedItem.Text : "<none selected>");
			};
			control.Expanding += (sender, e) =>
			{
				Log.Write(control, "Expanding, Item: {0}", e.Item.Text);
				e.Cancel = !(allowExpanding.Checked ?? true);
			};
			control.Expanded += (sender, e) =>
			{
				Log.Write(control, "Expanded, Item: {0}", e.Item.Text);
			};
			control.Collapsing += (sender, e) =>
			{
				Log.Write(control, "Collapsing, Item: {0}", e.Item.Text);
				e.Cancel = !(allowCollapsing.Checked ?? true);
			};
			control.Collapsed += (sender, e) =>
			{
				Log.Write(control, "Collapsed, Item: {0}", e.Item.Text);
			};
		}
	}
}