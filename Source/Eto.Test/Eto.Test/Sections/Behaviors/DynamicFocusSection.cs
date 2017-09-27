using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Dynamic Focus")]
	public class DynamicFocusSection : Panel
	{
		public DynamicFocusSection()
		{
			var content = new Panel();
			var focusControlCheckBox = new CheckBox { Text = "Focus Control", Checked = true };

			var addContentButton = new Button { Text = "Add Control" };
			var controls = new List<Func<Control>>
			{
				() => new TextBox(),
				() => new TextArea(),
				() => new CheckBox { Text = "A Check Box" },
				() => new RadioButton { Text = "A Radio Button" },
				() => new DropDown { Items = { "Item 1", "Item 2", "Item 3" } },
				() => new DateTimePicker(),
				() => new ColorPicker(),
				() => new PasswordBox(),
				() => new ListBox { Items = { "Item 1", "Item 2", "Item 3" } },
				() => new NumericStepper(),
			};

			var count = 0;
			addContentButton.Click += (sender, e) =>
			{
				Control control = controls[(count++) % controls.Count]();
				if (focusControlCheckBox.Checked ?? false)
					control.Focus();
				content.Content = new TableLayout(
					null,
					new Label { Text = string.Format("Control: {0}", control.GetType().Name) },
					new TableRow(control),
					null
				);
			};

			Content = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(10),
				Rows = {
					new StackLayout { Orientation = Orientation.Horizontal, Spacing = 5, Items = { addContentButton, focusControlCheckBox } },
					content
				}
			};
		}
	}
}
