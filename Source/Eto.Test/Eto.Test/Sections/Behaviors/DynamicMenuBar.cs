using System;
using Eto.Forms;
using System.Linq;


namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Dynamic MenuBar")]
	public class DynamicMenuBar : Panel
	{
		public DynamicMenuBar()
		{
			var count = 0;
			var menu = Application.Instance.MainForm.Menu;
			ISubmenu editMenu = menu.Items.GetSubmenu("Edit");

			// selection of which menu to add/remove
			var menuToEdit = new DropDown
			{
				DataStore = menu.Items.OfType<ISubmenu>().Union(new ISubmenu[] { menu }).ToList(),
				TextBinding = Binding.Delegate((MenuItem item) => item.Text, defaultGetValue: "Main Menu")
			};
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

			// layout of the form
			Content = new TableLayout
			{
				Rows =
				{
					null,
					TableLayout.Horizontal(null, new Label { Text = "Submenu to add to", VerticalAlign = VerticalAlign.Middle }, menuToEdit, null),
					TableLayout.Horizontal(null, addToEditMenu, removeFromEditMenu, null),
					null
				}
			};
		}
	}
}

