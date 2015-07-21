using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Addin.VisualStudio.Wizards
{
	public class BaseClassDefinition
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public Image Image { get; set; }
		public string Methods { get; set; }
		public string CodeContent { get; set; }
		public string JsonContent { get; set; }
		public string XamlAttributes { get; set; }
		public string XamlContent { get; set; }
	}

	public class BaseClassDialog : Dialog<BaseClassDefinition>
	{
		public BaseClassDialog(IEnumerable<BaseClassDefinition> baseClassDefinitions, string typeName)
		{
			Title = string.Format("Select {0} type", typeName);
			var layout = new TableLayout();
			layout.Padding = new Padding(10);
			layout.Spacing = new Size(5, 5);

			layout.Rows.Add(new Label { Text = string.Format("Select the type of {0} to create:", typeName) });

			foreach (var definition in baseClassDefinitions)
			{
				var current = definition;
				var button = new Button { Text = definition.Name, ToolTip = definition.Description };
				button.Click += (sender, e) => Close(current);
				layout.Rows.Add(button);
			}
			layout.Rows.Add(null);
			Content = layout;
		}
	}
}