using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(ComboBox))]
	public class ComboBoxSection : Scrollable
	{
		public ComboBoxSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow(new Label { Text = "Default" }, Default(), null);

			layout.AddRow(new Label { Text = "With Items" }, TableLayout.AutoSized(Items()));

			layout.AddRow(new Label { Text = "Set Initial Value" }, TableLayout.AutoSized(SetInitialValue()));

			layout.Add(null, null, true);

			Content = layout;
		}

		Control Default()
		{
			var control = new ComboBox();
			LogEvents(control);

			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };
			layout.Add(TableLayout.AutoSized(control));
			layout.AddSeparateRow(AddRowsButton(control), RemoveRowsButton(control), ClearButton(control), SetSelected(control), ClearSelected(control), null);
			layout.AddSeparateRow(GetEnabled(control), GetReadOnly(control), AutoComplete(control), ShowComboText(control), SetComboText(control), null);

			return layout;
		}

		Control AddRowsButton(DropDown list)
		{
			var control = new Button { Text = "Add Rows" };
			control.Click += delegate
			{
				for (int i = 0; i < 10; i++)
					list.Items.Add(new ListItem { Text = "Item " + list.Items.Count });
			};
			return control;
		}

		Control RemoveRowsButton(DropDown list)
		{
			var control = new Button { Text = "Remove Rows" };
			control.Click += delegate
			{
				if (list.SelectedIndex >= 0)
					list.Items.RemoveAt(list.SelectedIndex);
			};
			return control;
		}

		Control ClearButton(DropDown list)
		{
			var control = new Button { Text = "Clear" };
			control.Click += delegate
			{
				list.Items.Clear();
			};
			return control;
		}

		Control SetSelected(DropDown list)
		{
			var control = new Button { Text = "Set Selected" };
			control.Click += delegate
			{
				if (list.Items.Count > 0)
					list.SelectedIndex = new Random().Next(list.Items.Count) - 1;
			};
			return control;
		}


		Control ClearSelected(DropDown list)
		{
			var control = new Button { Text = "Clear Selected" };
			control.Click += delegate
			{
				list.SelectedIndex = -1;
			};
			return control;
		}

		DropDown Items()
		{
			var control = new ComboBox();
			LogEvents(control);
			for (int i = 0; i < 20; i++)
			{
				control.Items.Add(new ListItem { Text = "Item " + i });
			}
			return control;
		}

		DropDown Disabled()
		{
			var control = Items();
			control.Enabled = false;
			return control;
		}

		DropDown SetInitialValue()
		{
			var control = Items();
			control.SelectedKey = "Item 8";
			return control;
		}

		Control GetEnabled(ComboBox list)
		{
			var control = new CheckBox { Text = "Enabled" };
			control.CheckedBinding.Bind(list, l => l.Enabled);
			return control;
		}

		Control GetReadOnly(ComboBox list)
		{
			var control = new CheckBox { Text = "ReadOnly" };
			control.CheckedBinding.Bind(list, l => l.ReadOnly);
			return control;
		}

		Control AutoComplete(ComboBox list)
		{
			var control = new CheckBox { Text = "AutoComplete" };
			control.CheckedBinding.Bind(list, l => l.AutoComplete);
			return control;
		}

		Control ShowComboText(ComboBox list)
		{
			var control = new Button { Text = "Show ComboText" };
			control.Click += delegate
			{
				MessageBox.Show(list.Text);
			};
			return control;
		}

		Control SetComboText(ComboBox list)
		{
			var control = new Button { Text = "Set ComboText" };
			control.Click += delegate
			{
				list.Text = "New ComboText";
			};
			return control;
		}

		void LogEvents(ComboBox control)
		{
			control.SelectedIndexChanged += (sender, e) => Log.Write(control, "SelectedIndexChanged, Value: {0}", control.SelectedIndex);

			control.TextChanged += (sender, e) => Log.Write(control, "TextChanged, Value: {0}", control.Text);

			control.DropDownOpening += (sender, e) => Log.Write(control, "DropDownOpening");

			control.DropDownClosed += (sender, e) => Log.Write(control, "DropDownClosed");
		}
	}
}

