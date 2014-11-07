using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(DropDown))]
	public class ComboBoxSection : Scrollable
	{
		public ComboBoxSection()
		{
			var layout = new DynamicLayout();
			
			layout.AddRow(new Label { Text = "Default" }, Default(), null);
			
			layout.AddRow(new Label { Text = "With Items" }, TableLayout.AutoSized(Items()));

			layout.AddRow(new Label { Text = "Disabled" }, TableLayout.AutoSized(Disabled()));
			
			layout.AddRow(new Label { Text = "Set Initial Value" }, TableLayout.AutoSized(SetInitialValue()));
			
			layout.AddRow(new Label { Text = "EnumDropDown<Key>" }, TableLayout.AutoSized(EnumCombo()));

			layout.AddRow(new Label { Text = "ComboBox" }, TableLayout.AutoSized(ComboBox()));

			layout.AddRow(new Label { Text = "Editable ComboBox" }, TableLayout.AutoSized(EditableComboBox()));

			layout.Add(null, null, true);

			Content = layout;
		}

		Control Default()
		{
			var control = new DropDown();
			LogEvents(control);
			
			var layout = new DynamicLayout();
			layout.Add(TableLayout.AutoSized(control));
			layout.BeginVertical();
			layout.AddRow(null, AddRowsButton(control), RemoveRowsButton(control), ClearButton(control), SetSelected(control), ClearSelected(control), null);
			layout.EndVertical();
			
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
					list.SelectedIndex = new Random().Next(list.Items.Count - 1);
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
			var control = new DropDown();
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

		Control EnumCombo()
		{
			var control = new EnumDropDown<Keys>();
			LogEvents(control);
			control.SelectedKey = ((int)Keys.E).ToString();
			return control;
		}

		Control ComboBox()
		{
			var control = new ComboBox();
			control.IsEditable = false;
			LogEvents(control);
			for (int i = 0; i < 20; i++)
			{
				control.Items.Add(new ListItem { Text = "Item " + i });
			}
			return control;
		}

		Control EditableComboBox()
		{
			var control = new ComboBox();
			LogEvents(control);

			var layout = new DynamicLayout();
			layout.Add(TableLayout.AutoSized(control));
			layout.BeginVertical();
			layout.AddRow(null, AddRowsButton(control), RemoveRowsButton(control), ClearButton(control), SetSelected(control), ClearSelected(control), null);
			layout.AddRow(null, Editable(control), ShowComboText(control), SetComboText(control), SetComboFont(control), null);
			layout.EndVertical();

			return layout;
		}
		Control Editable(ComboBox list)
		{
			var control = new Button { Text = "Editable/Disable" };
			control.Click += delegate
			{
				list.IsEditable = !list.IsEditable;
			};
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

		Control SetComboFont(ComboBox list)
		{
			var control = new Button { Text = "Set ComboFont" };
			control.Click += delegate
			{
				list.Font = Fonts.Serif(10, FontStyle.Bold);
			};
			return control;
		}

		void LogEvents(DropDown control)
		{
			control.SelectedIndexChanged += delegate
			{
				Log.Write(control, "SelectedIndexChanged, Value: {0}", control.SelectedIndex);
			};
		}
	}
}

