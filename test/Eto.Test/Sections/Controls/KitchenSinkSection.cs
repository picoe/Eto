using System;
using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", "Kitchen Sink")]
	public class KitchenSinkSection : Panel
	{
		Image bitmap1 = TestIcons.TestImage.WithSize(16, 16);
		Icon icon1 = TestIcons.TestIcon.WithSize(16, 16);
		Icon icon2 = TestIcons.TestImage.WithSize(16, 16);

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

		Control DropDown()
		{
			var control = new DropDown();
			control.Items.Add(new ListItem { Text = "DropDown" });
			control.Items.Add(new ListItem { Text = "Item 2" });
			control.Items.Add(new ListItem { Text = "Item 3" });
			control.SelectedIndex = 0;
			return control;
		}

		Control ComboBox()
		{
			var control = new ComboBox();
			control.Items.Add(new ListItem { Text = "ComboBox" });
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
			control.Items.Add(new ImageListItem { Text = "ListBox", Image = icon1 });
			control.Items.Add(new ImageListItem { Text = "ListBox 2", Image = icon2 });
			control.Items.Add(new ImageListItem { Text = "ListBox 3", Image = icon1 });
			return control;
		}

		Control LeftPane()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };
			layout.BeginVertical();
			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Label", VerticalAlignment = VerticalAlignment.Center });
			layout.AddCentered(new Button { Text = "Button" });
			layout.AddCentered(new LinkButton { Text = "LinkButton" });
			layout.Add(new Label { Text = "ImageView", VerticalAlignment = VerticalAlignment.Center });
			layout.Add(new ImageView { Image = icon1, Size = new Size(64, 64) });
			layout.Add(null);
			layout.EndHorizontal();

			layout.EndBeginVertical();

			layout.AddSeparateRow(new CheckBox { Text = "CheckBox", ThreeState = true }, RadioButtons(), null);
			layout.AddSeparateRow(new TextBox { Text = "TextBox", Size = new Size(150, -1) }, "PasswordBox", new PasswordBox { Text = "PasswordBox", Size = new Size(150, -1) }, null);
			layout.AddSeparateRow(DropDown(), ComboBox(), null);
			layout.AddSeparateRow("Stepper", new Stepper(), "NumericStepper", new NumericStepper { Value = 50, DecimalPlaces = 1 }, new TextStepper { Text = "TextStepper" }, null);

			layout.BeginVertical();
			layout.BeginHorizontal();
			layout.BeginVertical();
			layout.AddSeparateRow("DateTimePicker", new DateTimePicker { Value = DateTime.Now }, null);
			layout.AddSeparateRow(new TextArea { Text = "TextArea", Size = new Size(150, 50) }, CreateRichTextArea(), null);
			layout.AddSeparateRow(ListBox(), new GroupBox { Text = "GroupBox", Content = new Label { Text = "I'm in a group box" } }, null);
			layout.EndVertical();
			layout.AddSeparateColumn("Calendar", new Calendar(), null);
			layout.EndHorizontal();
			layout.EndVertical();

			layout.AddSeparateRow("Slider", new Slider { Value = 50, TickFrequency = 10 });
			layout.AddSeparateRow("ProgressBar", new ProgressBar { Value = 25, Width = 100 }, "Spinner", new Spinner { Enabled = true }, null);
			layout.EndVertical();

			layout.EndVertical();
			layout.Add(null);

			return layout;
		}

		RichTextArea CreateRichTextArea()
		{
			var richTextArea = new RichTextArea { Text = "RichTextArea", Size = new Size(150, 50) };
			richTextArea.Buffer.SetBold(new Range<int>(0, 3), true);
			richTextArea.Buffer.SetForeground(new Range<int>(0, 3), Colors.Blue);
			richTextArea.Buffer.SetItalic(new Range<int>(4, 7), true);
			richTextArea.Buffer.SetStrikethrough(new Range<int>(4, 7), true);
			richTextArea.Buffer.SetForeground(new Range<int>(4, 7), Colors.Green);
			richTextArea.Buffer.SetUnderline(new Range<int>(8, 11), true);
			richTextArea.Buffer.SetForeground(new Range<int>(8, 11), Colors.Red);
			return richTextArea;
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

			control.Columns.Add(new GridColumn { DataCell = new ImageViewCell(0), HeaderText = "ImageViewCell" });
			control.Columns.Add(new GridColumn { DataCell = new CheckBoxCell(1), HeaderText = "CheckBoxCell", Editable = true });
			control.Columns.Add(new GridColumn { DataCell = new TextBoxCell(2), HeaderText = "TextBoxCell", Editable = true });
			control.Columns.Add(new GridColumn { DataCell = new ComboBoxCell(3) { DataStore = ComboCellItems() }, HeaderText = "ComboBoxCell", Editable = true });

			var items = new List<GridItem>();
			items.Add(new GridItem(bitmap1, true, "GridView 1", "1"));
			items.Add(new GridItem(icon1, false, "GridView 2", "2"));
			items.Add(new GridItem(bitmap1, null, "GridView 3", "3"));

			control.DataStore = items;

			return control;
		}

		IEnumerable<ITreeGridItem> TreeChildren(int level = 0)
		{
			if (level > 4)
				yield break;
			yield return new TreeGridItem(TreeChildren(level + 1), bitmap1, "TreeGridView 1", true, "1") { Expanded = level < 2 };
			yield return new TreeGridItem(icon1, "TreeGridView 2", false, "2");
			yield return new TreeGridItem(TreeChildren(level + 1), bitmap1, "TreeGridView 3", null, "3");
		}

		Control TreeView()
		{
			var control = new TreeGridView { Size = new Size(-1, 150) };

			control.Columns.Add(new GridColumn { DataCell = new ImageTextCell(0, 1), HeaderText = "ImageTextCell" });
			control.Columns.Add(new GridColumn { DataCell = new CheckBoxCell(2), HeaderText = "CheckBoxCell", Editable = true, AutoSize = true });
			control.Columns.Add(new GridColumn { DataCell = new ComboBoxCell(3) { DataStore = ComboCellItems() }, HeaderText = "ComboBoxCell", Editable = true });

			control.DataStore = new TreeGridItemCollection(TreeChildren());

			return control;
		}

		Control WebView()
		{
			try
			{
				var control = new WebView { Size = new Size(-1, 100) };
				control.LoadHtml("<html><head><title>Hello</title></head><body><h1>WebView</h1><p>This is a web view loaded with a html string</p></body>");
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

