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

			var count = 0;
			addContentButton.Click += (sender, e) =>
			{
				Control control;
				switch((count++) % 9)
				{
					case 0: control = new TextBox(); break;
					case 1: control = new TextArea(); break;
					case 2: control = new CheckBox { Text = "A Check Box" }; break;
					case 3: control = new RadioButton { Text = "A Radio Button" }; break;
					case 4: control = new ComboBox { Items = { "Item 1", "Item 2", "Item 3" } }; break;
					case 5: control = new DateTimePicker(); break;
					case 6: control = new ColorPicker(); break;
					case 7: control = new PasswordBox(); break;
					case 8: control = new ListBox { Items = { "Item 1", "Item 2", "Item 3" } }; break;
					case 9: control = new PasswordBox(); break;
					default: throw new InvalidOperationException();
				}
				if (focusControlCheckBox.Checked ?? false)
					control.Focus();
				content.Content = new TableLayout(
					null,
					new Label { Text = string.Format("Control: {0}", control.GetType().Name) },
					new TableRow(control),
					null
				);
			};

			Content = new TableLayout(
				new TableLayout(new TableRow(null, addContentButton, focusControlCheckBox, null)),
				content
				);
		}
    }
}
