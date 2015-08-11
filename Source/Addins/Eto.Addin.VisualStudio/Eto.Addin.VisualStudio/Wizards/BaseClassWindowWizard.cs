using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Addin.VisualStudio.Wizards
{
	public class BaseClassWindowWizard : BaseClassWizard
	{
		protected override string TypeName { get { return "window"; } }

		protected override IEnumerable<BaseClassDefinition> Definitions
		{
			get
			{
				yield return new BaseClassDefinition
				{
					Name = "Form",
					Description = "A modeless form window",
					CodeContent = @"
Title = ""My Form"";
ClientSize = new Size(200, 200);

Content = new Label { Text = ""Some Content"" };",
					JsonContent = @"
    ""Title"": ""My Form"",
    ""ClientSize"": ""200, 200"",",
					XamlAttributes = @"
  Title=""My Form""
  ClientSize=""200, 200"""
				};

				yield return new BaseClassDefinition
				{
					Name = "Dialog",
					Description = "A modal dialog window",
					CodeContent = @"
Title = ""My Dialog"";

// buttons
DefaultButton = new Button { Text = ""OK"" };

AbortButton = new Button { Text = ""C&ancel"" };
AbortButton.Click += (sender, e) => Close();

var buttons = new TableLayout { Rows = { new TableRow(null, DefaultButton, AbortButton) }, Spacing = new Size(5, 5) };

Content = new TableLayout
{
	Padding = new Padding(10),
	Rows = 
	{
		new TableRow { Cells = { new Label { Text = ""Some Content"" } }, ScaleHeight = true },
		buttons
	}
};",
					JsonContent = @"
    ""Title"": ""My Dialog"",
    ""ClientSize"": ""200, 200"",",
					XamlAttributes = @"
  Title=""My Dialog""
  ClientSize=""200, 200"""
				};
			}
		}
	}
}
