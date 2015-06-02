using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(DropDown))]
	public class DropDownSection : Scrollable
	{
		public DropDownSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow("Default", Default(), null);

			layout.AddRow("With Items", TableLayout.AutoSized(Items()));

			layout.AddRow("Disabled", TableLayout.AutoSized(Disabled()));

			layout.AddRow("Set Initial Value", TableLayout.AutoSized(SetInitialValue()));

			layout.AddRow("EnumDropDown<Key>", TableLayout.AutoSized(EnumCombo()));

			layout.Add(null, null, true);

			Content = layout;
		}

		Control Default()
		{
			var control = new DropDown();
			LogEvents(control);

			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };
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

		void LogEvents(DropDown control)
		{
			control.SelectedIndexChanged += delegate
			{
				Log.Write(control, "SelectedIndexChanged, Value: {0}", control.SelectedIndex);
			};
		}
	}
}

