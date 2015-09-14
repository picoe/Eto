using System;
using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", "Kitchen Sink")]
	public class KitchenSinkSection : Panel
	{
		Bitmap bitmap1 = TestIcons.TestImage;
		Icon icon1 = TestIcons.TestIcon;

		public KitchenSinkSection()
		{
			Content = Tabs();
		}

		Control Tabs()
		{
			var control = new TabControl();
			control.Pages.Add(MainContent(new TabPage { Text = "Tab 1", Image = icon1, Padding = new Padding(10) }));
			control.Pages.Add(new TabPage { Text = "Tab 2", Image = bitmap1 });
			return control;
		}

		T MainContent<T>(T container)
			where T : Panel
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };
			layout.AddRow(LeftPane(), RightPane());
			container.Content = layout;
			return container;
		}

		Control ComboBox()
		{
			var control = new DropDown();
			control.Items.Add(new ListItem { Text = "Combo Box" });
			control.Items.Add(new ListItem { Text = "Item 2" });
			control.Items.Add(new ListItem { Text = "Item 3" });
			control.SelectedIndex = 0;
			return control;
		}

		Control RadioButtons()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };
			RadioButton controller;
			layout.AddRow(controller = new RadioButton { Text = "Radio 1", Checked = true }, new RadioButton(controller) { Text = "Radio 2" });
			return layout;
		}

		Control ListBox()
		{
			var control = new ListBox { Size = new Size(150, 50) };
			control.Items.Add(new ImageListItem { Text = "Simple List Box 1", Image = bitmap1 });
			control.Items.Add(new ImageListItem { Text = "Simple List Box 2", Image = icon1 });
			control.Items.Add(new ImageListItem { Text = "Simple List Box 3", Image = bitmap1 });
			return control;
		}

		Control LeftPane()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };
			layout.BeginVertical();
			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Label", VerticalAlignment = VerticalAlignment.Center });
			layout.AddAutoSized(new Button { Text = "Button Control" }, centered: true);
			layout.Add(new ImageView { Image = icon1, Size = new Size(64, 64) });
			layout.Add(null);
			layout.EndHorizontal();
			layout.EndBeginVertical();
			layout.AddRow(new CheckBox { Text = "Check Box (/w three state)", ThreeState = true, Checked = null }, RadioButtons(), null);
			layout.EndBeginVertical();
			layout.AddRow(new TextBox { Text = "Text Box", Size = new Size(150, -1) }, new PasswordBox { Text = "Password Box", Size = new Size(150, -1) }, null);
			layout.EndBeginVertical();
			layout.AddRow(ComboBox(), new DateTimePicker { Value = DateTime.Now }, null);
			layout.EndBeginVertical();
			layout.AddRow(new NumericUpDown { Value = 50 }, null);
			layout.EndBeginVertical();
			layout.AddRow(ListBox(), new TextArea { Text = "Text Area", Size = new Size(150, 50) }, null);
			layout.EndBeginVertical();
			layout.AddRow(new Slider { Value = 50, TickFrequency = 10 });
			layout.EndBeginVertical();
			layout.AddRow(new ProgressBar { Value = 25 });
			layout.EndBeginVertical();
			layout.AddRow(new GroupBox { Text = "Group Box", Content = new Label { Text = "I'm in a group box" } });

			layout.EndBeginVertical();


			layout.EndVertical();
			layout.Add(null);

			return layout;
		}

		IEnumerable<object> ComboCellItems()
		{
			var items = new ListItemCollection();
			items.Add(new ListItem { Text = "Grid Combo 1", Key = "1" });
			items.Add(new ListItem { Text = "Grid Combo 2", Key = "2" });
			items.Add(new ListItem { Text = "Grid Combo 3", Key = "3" });
			return items;
		}

		Control GridView()
		{
			var control = new GridView { Size = new Size(-1, 150) };

			control.Columns.Add(new GridColumn { DataCell = new ImageViewCell(0), HeaderText = "Image" });
			control.Columns.Add(new GridColumn { DataCell = new CheckBoxCell(1), HeaderText = "Check", Editable = true });
			control.Columns.Add(new GridColumn { DataCell = new TextBoxCell(2), HeaderText = "Text", Editable = true });
			control.Columns.Add(new GridColumn { DataCell = new ComboBoxCell(3) { DataStore = ComboCellItems() }, HeaderText = "Combo", Editable = true });

			var items = new List<GridItem>();
			items.Add(new GridItem(bitmap1, true, "Text in Grid 1", "1"));
			items.Add(new GridItem(icon1, false, "Text in Grid 2", "2"));
			items.Add(new GridItem(bitmap1, null, "Text in Grid 3", "3"));

			control.DataStore = items;

			return control;
		}

		IEnumerable<ITreeGridItem> TreeChildren(int level = 0)
		{
			if (level > 4)
				yield break;
			yield return new TreeGridItem(TreeChildren(level + 1), bitmap1, "Text in Tree 1", true, "1") { Expanded = level < 2 };
			yield return new TreeGridItem(icon1, "Text in Tree 2", false, "2");
			yield return new TreeGridItem(TreeChildren(level + 1), bitmap1, "Text in Tree 3", null, "3");
		}

		Control TreeView()
		{
			var control = new TreeGridView { Size = new Size(-1, 150) };

			control.Columns.Add(new GridColumn { DataCell = new ImageTextCell(0, 1), HeaderText = "Image and Text" });
			control.Columns.Add(new GridColumn { DataCell = new CheckBoxCell(2), HeaderText = "Check", Editable = true, AutoSize = true });
			control.Columns.Add(new GridColumn { DataCell = new ComboBoxCell(3) { DataStore = ComboCellItems() }, HeaderText = "Combo", Editable = true });

			control.DataStore = new TreeGridItemCollection(TreeChildren());

			return control;
		}

		Control WebView()
		{
			try
			{
				var control = new WebView { Size = new Size(-1, 100) };
				control.LoadHtml("<html><head><title>Hello</title></head><body><h1>Web View</h1><p>This is a web view loaded with a html string</p></body>");
				return control;
			}
			catch (Exception)
			{
				var control = new Label
				{
					Text = string.Format("WebView not supported on this platform with the {0} generator", Platform.ID),
					BackgroundColor = Colors.Red,
					TextAlignment = TextAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					TextColor = Colors.White
				};
				if (Platform.IsGtk)
					Log.Write(this, "You must install webkit-sharp for WebView to work under GTK. Note that GTK does not support webkit-sharp on any platform other than Linux.");
				return control;
			}
		}

		Control RightPane()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };
			layout.DefaultPadding = Padding.Empty;

			layout.Add(WebView());
			layout.Add(GridView());
			layout.Add(TreeView());

			return layout;
		}
	}
}

