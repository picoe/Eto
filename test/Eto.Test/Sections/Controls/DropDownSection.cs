using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(DropDown))]
	public class DropDownSection : Scrollable
	{
		bool AddWithImages { get; set; }

		public DropDownSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow("Default", Default(), null);

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
			layout.AddSeparateRow(null, AddRowsButton(control), AddWithImagesCheckBox(), RemoveRowsButton(control), ClearButton(control), null);
			layout.AddSeparateRow(null, EnabledCheckBox(control), SetSelected(control), ClearSelected(control), null);

			return layout;
		}

		Control EnabledCheckBox(DropDown list)
		{
			var control = new CheckBox { Text = "Enabled" };
			control.CheckedBinding.Bind(list, l => l.Enabled);
			return control;
		}

		Control AddWithImagesCheckBox()
		{
			var control = new CheckBox { Text = "Add with images" };
			control.CheckedBinding.Bind(this, l => l.AddWithImages);
			return control;
		}


		Control AddRowsButton(DropDown list)
		{
			var control = new Button { Text = "Add Rows" };
			control.Click += (sender, e) =>
			{
				var image1 = TestIcons.Logo.WithSize(32, 32);
				var image2 = TestIcons.TestIcon.WithSize(16, 16);
				for (int i = 0; i < 10; i++)
				{
					if (AddWithImages)
						list.Items.Add(new ImageListItem { Text = "Item " + list.Items.Count, Image = i % 2 == 0 ? image1 : image2 });
					else
						list.Items.Add(new ListItem { Text = "Item " + list.Items.Count });
				}
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
			control.SelectedIndexChanged += (sender, e) =>  Log.Write(control, "SelectedIndexChanged, Value: {0}", control.SelectedIndex);

			control.DropDownOpening += (sender, e) => Log.Write(control, "DropDownOpening");
			control.DropDownClosed += (sender, e) => Log.Write(control, "DropDownClosed");
		}
	}
}

