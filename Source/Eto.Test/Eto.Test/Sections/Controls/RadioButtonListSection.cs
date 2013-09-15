using System;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	public class RadioButtonListSection : Scrollable
	{
		public RadioButtonListSection()
		{
			var layout = new DynamicLayout();
			
			layout.AddRow(new Label { Text = "Default" }, Default(), null);
			
			layout.AddRow(new Label { Text = "With Items" }, TableLayout.AutoSized(Items()));

			layout.AddRow(new Label { Text = "Disabled" }, TableLayout.AutoSized(Disabled()));
			
			layout.AddRow(new Label { Text = "Set Initial Value" }, TableLayout.AutoSized(SetInitialValue()));
			
			layout.AddRow(new Label { Text = "EnumRadioButtonList<TestEnum>" }, TableLayout.AutoSized(EnumCombo()));

			layout.AddRow(new Label { Text = "Vertical" }, TableLayout.AutoSized(Items(RadioButtonListOrientation.Vertical)));

			layout.Add(null, null, true);

			Content = layout;
		}

		Control Default()
		{
			var control = new RadioButtonList();
			LogEvents(control);
			
			var layout = new DynamicLayout();
			layout.Add(TableLayout.AutoSized(control));
			layout.BeginVertical();
			layout.AddRow(null, AddRowsButton(control), RemoveRowsButton(control), ClearButton(control), Orientation(control), null);
			layout.EndVertical();
			
			return layout;
		}

		Control AddRowsButton(RadioButtonList list)
		{
			var control = new Button { Text = "Add" };
			control.Click += delegate
			{
				for (int i = 0; i < 1; i++)
					list.Items.Add(new ListItem { Text = "Item " + list.Items.Count });
			};
			return control;
		}

		Control RemoveRowsButton(RadioButtonList list)
		{
			var control = new Button { Text = "Remove" };
			control.Click += delegate
			{
				if (list.SelectedIndex >= 0)
					list.Items.RemoveAt(list.SelectedIndex);
			};
			return control;
		}

		Control ClearButton(RadioButtonList list)
		{
			var control = new Button { Text = "Clear" };
			control.Click += delegate
			{
				list.Items.Clear();
			};
			return control;
		}

		Control Orientation(RadioButtonList list)
		{
			var control = new EnumComboBox<RadioButtonListOrientation>();
			control.SelectedValue = list.Orientation;
			control.SelectedValueChanged += delegate
			{
				list.Orientation = control.SelectedValue;
			};
			return TableLayout.AutoSized(control, centered: true);
		}

		RadioButtonList Items(RadioButtonListOrientation? orientation = null)
		{
			var control = new RadioButtonList();
			if (orientation != null)
				control.Orientation = orientation.Value;
			LogEvents(control);
			for (int i = 0; i < 5; i++)
			{
				control.Items.Add(new ListItem { Text = "Item " + i });
			}
			return control;
		}

		RadioButtonList Disabled()
		{
			var control = Items();
			control.Enabled = false;
			return control;
		}

		RadioButtonList SetInitialValue()
		{
			var control = Items();
			control.SelectedKey = "Item 3";
			return control;
		}

		public enum TestEnum
		{
			Enum1,
			Enum2,
			Enum3,
			Enum4
		}

		Control EnumCombo()
		{
			var control = new EnumRadioButtonList<TestEnum>();
			LogEvents(control);
			control.SelectedValue = TestEnum.Enum2;
			return control;
		}

		void LogEvents(RadioButtonList control)
		{
			control.SelectedIndexChanged += delegate
			{
				Log.Write(control, "SelectedIndexChanged, Value: {0}", control.SelectedIndex);
			};
		}
	}
}

