using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(CheckBoxList))]
	public class CheckBoxListSection : Scrollable
	{
		public CheckBoxListSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow(new Label { Text = "Default" }, Default(), null);

			layout.AddRow(new Label { Text = "With Items" }, TableLayout.AutoSized(Items()));

			layout.AddRow(new Label { Text = "Disabled" }, TableLayout.AutoSized(Disabled()));

			layout.AddRow(new Label { Text = "Set Initial Value" }, TableLayout.AutoSized(SetInitialValue()));

			layout.AddRow(new Label { Text = "EnumCheckBoxList<TestEnum>" }, TableLayout.AutoSized(EnumCombo()));

			layout.AddRow(new Label { Text = "Vertical" }, TableLayout.AutoSized(Items(Orientation.Vertical)));

			layout.Add(null, null, true);

			Content = layout;
		}

		Control Default()
		{
			var control = new CheckBoxList();
			LogEvents(control);

			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };
			layout.Add(TableLayout.AutoSized(control));
			layout.BeginVertical();
			layout.AddRow(null, AddRowsButton(control), RemoveRowsButton(control), ClearButton(control), OrientationDropDown(control), TextColorControl(control), null);
			layout.EndVertical();

			return layout;
		}

		Control AddRowsButton(CheckBoxList list)
		{
			var control = new Button { Text = "Add" };
			control.Click += delegate
			{
				for (int i = 0; i < 1; i++)
					list.Items.Add(new ListItem { Text = "Item " + list.Items.Count });
			};
			return control;
		}

		Control RemoveRowsButton(CheckBoxList list)
		{
			var control = new Button { Text = "Remove" };
			control.Click += delegate
			{
				foreach (var item in list.SelectedValues)
					list.Items.Remove((ListItem)item);
			};
			return control;
		}

		Control ClearButton(CheckBoxList list)
		{
			var control = new Button { Text = "Clear" };
			control.Click += delegate
			{
				list.Items.Clear();
			};
			return control;
		}

		Control OrientationDropDown(CheckBoxList list)
		{
			var control = new EnumDropDown<Orientation>();
			control.SelectedValue = list.Orientation;
			control.SelectedValueChanged += delegate
			{
				list.Orientation = control.SelectedValue;
			};
			return TableLayout.AutoSized(control, centered: true);
		}

		Control TextColorControl(CheckBoxList list)
		{
			var control = new ColorPicker();
			control.Value = list.TextColor;
			control.ValueChanged += (sender, e) =>
			{
				list.TextColor = control.Value;
			};
			return control;
		}

		CheckBoxList Items(Orientation? orientation = null)
		{
			var control = new CheckBoxList();
			if (orientation != null)
				control.Orientation = orientation.Value;
			LogEvents(control);
			for (int i = 0; i < 5; i++)
			{
				control.Items.Add(new ListItem { Text = "Item " + i });
			}
			return control;
		}

		CheckBoxList Disabled()
		{
			var control = Items();
			control.Enabled = false;
			return control;
		}

		CheckBoxList SetInitialValue()
		{
			var control = Items();
			control.SelectedKeys = new[] { "Item 3" };
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
			var control = new EnumCheckBoxList<TestEnum>();
			LogEvents(control);
			control.SelectedValues = new[] { TestEnum.Enum2 };
			return control;
		}

		void LogEvents(CheckBoxList control)
		{
			control.SelectedKeysChanged += (sender, e) => Log.Write(control, $"SelectedKeysChanged, Value: {string.Join(", ", control.SelectedKeys)}");
		}
	}
}

