using System;
using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Test.Sections.Controls
{
	public class KitchenSinkSection : Panel
	{
		Bitmap bitmap1 = Bitmap.FromResource ("Eto.Test.TestImage.png");
		Icon icon1 = Icon.FromResource ("Eto.Test.TestIcon.ico");
		
		public KitchenSinkSection ()
		{
			var layout = new DynamicLayout (this);

			layout.Add (Tabs ());
		}
		
		Control Tabs ()
		{
			var control = new TabControl ();
			control.TabPages.Add (MainContent (new TabPage{ Text = "Tab 1", Image = icon1 }));
			control.TabPages.Add (new TabPage{ Text = "Tab 2", Image = bitmap1 });
			return control;
		}
		
		T MainContent<T> (T container)
			where T: Container
		{
			var layout = new DynamicLayout (container);
			layout.AddRow (LeftPane (), RightPane ());
			return container;
		}
		
		Control ComboBox ()
		{
			var control = new ComboBox ();
			control.Items.Add (new ListItem { Text = "Combo Box"});
			control.Items.Add (new ListItem { Text = "Item 2"});
			control.Items.Add (new ListItem { Text = "Item 3"});
			control.SelectedIndex = 0;
			return control;
		}
		
		Control RadioButtons ()
		{
			var layout = new DynamicLayout (new Panel ());
			RadioButton controller;
			layout.AddRow (controller = new RadioButton{ Text = "Radio 1", Checked = true }, new RadioButton (controller) { Text = "Radio 2"});
			return layout.Container;
		}
		
		Control ListBox ()
		{
			var control = new ListBox { Size = new Size (150, 50) };
			control.Items.Add (new ImageListItem{ Text = "Simple List Box 1", Image = bitmap1 });
			control.Items.Add (new ImageListItem{ Text = "Simple List Box 2", Image = icon1 });
			control.Items.Add (new ImageListItem{ Text = "Simple List Box 3", Image = bitmap1 });
			return control;
		}
		
		Control LeftPane ()
		{
			var layout = new DynamicLayout (new Panel ());
			layout.DefaultPadding = Padding.Empty;
			layout.BeginVertical ();
			layout.AddRow (new Label { Text = "Label", VerticalAlign = VerticalAlign.Middle}, new Button{ Text = "Button Control"}, new ImageView { Image = icon1 }, null);
			layout.EndBeginVertical ();
			layout.AddRow (new CheckBox { Text = "Check Box (/w three state)", ThreeState = true, Checked = null }, RadioButtons (), null);
			layout.EndBeginVertical ();
			layout.AddRow (new TextBox { Text = "Text Box", Size = new Size (150, -1) }, new PasswordBox { Text = "Password Box", Size = new Size (150, -1)}, null);
			layout.EndBeginVertical ();
			layout.AddRow (ComboBox (), new DateTimePicker{ Value = DateTime.Now }, null);
			layout.EndBeginVertical ();
			layout.AddRow (new NumericUpDown { Value = 50 }, null);
			layout.EndBeginVertical ();
			layout.AddRow (ListBox (), new TextArea{ Text = "Text Area", Size = new Size (150, 50)}, null);
			layout.EndBeginVertical ();
			layout.AddRow (new Slider { Value = 50, TickFrequency = 10 });
			layout.EndBeginVertical ();
			layout.AddRow (new ProgressBar{ Value = 25 });
			layout.EndBeginVertical ();
			layout.AddRow (new GroupBox{ Text = "Group Box" }.AddDockedControl (new Label { Text = "I'm in a group box" }));
			
			layout.EndBeginVertical ();
			
			
			layout.EndVertical ();
			layout.Add (null);
			
			return layout.Container;
		}
		
		IListStore ComboCellItems ()
		{
			var items = new ListItemCollection ();
			items.Add (new ListItem{ Text = "Grid Combo 1", Key = "1" });
			items.Add (new ListItem{ Text = "Grid Combo 2", Key = "2" });
			items.Add (new ListItem{ Text = "Grid Combo 3", Key = "3" });
			return items;
		}
		
		Control GridView ()
		{
			var control = new GridView { Size = new Size(-1, 150)};
			
			control.Columns.Add (new GridColumn{ DataCell = new ImageCell(), HeaderText = "Image" });
			control.Columns.Add (new GridColumn{ DataCell = new CheckBoxCell(), HeaderText = "Check", Editable = true });
			control.Columns.Add (new GridColumn{ DataCell = new TextCell(), HeaderText = "Text", Editable = true });
			control.Columns.Add (new GridColumn{ DataCell = new ComboBoxCell{ DataStore = ComboCellItems() }, HeaderText = "Combo", Editable = true });
			
			var items = new GridItemCollection ();
			items.Add (new GridItem(bitmap1, true, "Text in Grid 1", "1"));
			items.Add (new GridItem(icon1, false, "Text in Grid 2", "2"));
			items.Add (new GridItem(bitmap1, null, "Text in Grid 3", "3"));
			
			control.DataStore = items;
			
			return control;
		}
		
		IEnumerable<ITreeItem> TreeChildren(int level = 0)
		{
			if (level > 4) yield break;
			yield return new TreeItem(TreeChildren(level + 1), new object[] { bitmap1, "Text in Tree 1"}, true, "1") { Expanded = level < 2 };
			yield return new TreeItem(new object[] { icon1, "Text in Tree 2"}, false, "2");
			yield return new TreeItem(TreeChildren(level + 1), new object[] { bitmap1, "Text in Tree 3"}, null, "3");
		}

		Control TreeView ()
		{
			var control = new TreeView { Size = new Size(-1, 150)};
			
			control.Columns.Add (new TreeColumn{ DataCell = new ImageTextCell(), HeaderText = "Image and Text" });
			control.Columns.Add (new TreeColumn{ DataCell = new CheckBoxCell(), HeaderText = "Check", Editable = true, AutoSize = true });
			control.Columns.Add (new TreeColumn{ DataCell = new ComboBoxCell{ DataStore = ComboCellItems() }, HeaderText = "Combo", Editable = true });
			
			control.DataStore = new TreeItemCollection(TreeChildren ());
			
			return control;
		}
		
		Control RightPane ()
		{
			var layout = new DynamicLayout (new Panel ());
			layout.DefaultPadding = Padding.Empty;
			
			layout.Add (new WebView { Url = new Uri("https://github.com/picoe/Eto/wiki"), Size = new Size(-1, 150) });
			layout.Add (GridView());
			layout.Add (TreeView());
			
			return layout.Container;
		}
	}
}

