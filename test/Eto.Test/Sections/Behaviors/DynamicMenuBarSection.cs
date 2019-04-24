using System;
using Eto.Forms;
using System.Linq;
using Eto.Drawing;
using System.ComponentModel;


namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Dynamic MenuBar")]
	public class DynamicMenuBarSection : Panel, INotifyPropertyChanged
	{
		MenuItem _CurrentMenuItem;

		public event PropertyChangedEventHandler PropertyChanged;

		MenuItem CurrentMenuItem
		{
			get => _CurrentMenuItem;
			set
			{
				_CurrentMenuItem = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentMenuItem)));
			}
		}

		public DynamicMenuBarSection()
		{
			var count = 0;
			var menu = Application.Instance.MainForm.Menu;
			ISubmenu editMenu = menu.Items.GetSubmenu("Edit");

			// selection of which menu to add/remove
			var menuToEdit = new DropDown
			{
				DataStore = menu.Items.OfType<ISubmenu>().Union(new ISubmenu[] { menu }).ToList(),
				ItemTextBinding = Binding.Delegate((MenuItem item) => item.Text, defaultGetValue: "Main Menu")
			};
			menuToEdit.SelectedValueBinding.Bind(this, t => t.CurrentMenuItem);
			menuToEdit.SelectedValueBinding.Bind(() => editMenu, v => editMenu = v as ISubmenu);

			// tag to identify items that we've added
			var dynamicTag = new object();

			// button to add a new item
			var addToEditMenu = new Button { Text = "Add MenuItem" };
			addToEditMenu.Click += (sender, e) =>
			{
				var itemNumber = ++count;
				var item = new ButtonMenuItem { Text = "Dynamic Menu Item " + itemNumber, Tag = dynamicTag };
				item.Click += (s, ee) => Log.Write(item, "Clicked " + itemNumber);
				editMenu.Items.Add(item);
			};

			// button to remove the item
			var removeFromEditMenu = new Button { Text = "Remove MenuItem" };
			removeFromEditMenu.Click += (sender, e) =>
			{
				var item = editMenu.Items.LastOrDefault(r => Equals(r.Tag, dynamicTag));
				if (item != null)
				{
					editMenu.Items.Remove(item);
				}
			};

			var visibilityCheckBox = new CheckBox { Text = "MenuItem.Visible" };
			visibilityCheckBox.CheckedBinding.Bind(this, t => t.CurrentMenuItem.Visible);


			// layout of the form
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };

			layout.BeginCentered(yscale: true);
			layout.AddSeparateRow("Submenu to add to", menuToEdit);
			layout.AddSeparateRow(addToEditMenu, removeFromEditMenu);
			layout.AddSeparateRow(visibilityCheckBox);

			layout.EndCentered();

			Content = layout;
		}
	}
}

