using System;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(ComboBox))]
	public class ComboBoxSection : Scrollable
	{
		public ComboBoxSection()
		{
			var layout = new DynamicLayout();
			
			layout.AddRow(new Label { Text = "Default" }, Default(), null);
			
			layout.AddRow(new Label { Text = "With Items" }, TableLayout.AutoSized(Items()));

			layout.AddRow(new Label { Text = "Disabled" }, TableLayout.AutoSized(Disabled()));
			
			layout.AddRow(new Label { Text = "Set Initial Value" }, TableLayout.AutoSized(SetInitialValue()));
			
			layout.AddRow(new Label { Text = "EnumComboBox<Key>" }, TableLayout.AutoSized(EnumCombo()));

			layout.Add(null, null, true);

			Content = layout;
		}

		Control Default()
		{
			var control = new ComboBox();
			LogEvents(control);
			
			var layout = new DynamicLayout();
			layout.Add(TableLayout.AutoSized(control));
			layout.BeginVertical();
			layout.AddRow(null, AddRowsButton(control), RemoveRowsButton(control), ClearButton(control), SetSelected(control), ClearSelected(control), null);
			layout.EndVertical();
			
			return layout;
		}

		Control AddRowsButton(ComboBox list)
		{
			var control = new Button { Text = "Add Rows" };
			control.Click += delegate
			{
				for (int i = 0; i < 10; i++)
					list.Items.Add(new ListItem { Text = "Item " + list.Items.Count });
			};
			return control;
		}

		Control RemoveRowsButton(ComboBox list)
		{
			var control = new Button { Text = "Remove Rows" };
			control.Click += delegate
			{
				if (list.SelectedIndex >= 0)
					list.Items.RemoveAt(list.SelectedIndex);
			};
			return control;
		}

		Control ClearButton(ComboBox list)
		{
			var control = new Button { Text = "Clear" };
			control.Click += delegate
			{
				list.Items.Clear();
			};
			return control;
		}

		Control SetSelected(ComboBox list)
		{
			var control = new Button { Text = "Set Selected" };
			control.Click += delegate
			{
				if (list.Items.Count > 0)
					list.SelectedIndex = new Random().Next(list.Items.Count - 1);
			};
			return control;
		}


		Control ClearSelected(ComboBox list)
		{
			var control = new Button { Text = "Clear Selected" };
			control.Click += delegate
			{
				list.SelectedIndex = -1;
			};
			return control;
		}

		ComboBox Items()
		{
			var control = new ComboBox();
			LogEvents(control);
			for (int i = 0; i < 20; i++)
			{
				control.Items.Add(new ListItem { Text = "Item " + i });
			}
			return control;
		}

		ComboBox Disabled()
		{
			var control = Items();
			control.Enabled = false;
			return control;
		}

		ComboBox SetInitialValue()
		{
			var control = Items();
			control.SelectedKey = "Item 8";
			return control;
		}

		Control EnumCombo()
		{
			var control = new EnumComboBox<Keys>();
			LogEvents(control);
			control.SelectedKey = ((int)Keys.E).ToString();
			return control;
		}

		void LogEvents(ComboBox control)
		{
			control.SelectedIndexChanged += delegate
			{
				Log.Write(control, "SelectedIndexChanged, Value: {0}", control.SelectedIndex);
			};
		}
	}
}

