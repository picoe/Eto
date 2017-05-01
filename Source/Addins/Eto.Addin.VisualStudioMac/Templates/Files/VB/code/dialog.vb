using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;

namespace ${Namespace}
{	
	public class ${EscapedIdentifier} : Dialog
	{	
		public ${EscapedIdentifier}()
		{
			Title = "My ${EscapedIdentifier} dialog";

			// buttons
			DefaultButton = new Button { Text = "OK" };

			AbortButton = new Button { Text = "C&ancel" };
			AbortButton.Click += (sender, e) => Close();

			var buttons = new TableLayout { Rows = { new TableRow(null, DefaultButton, AbortButton) }, Spacing = new Size(5, 5) };

			Content = new TableLayout
			{
				Padding = new Padding(10),
				Rows = 
				{
					new Label { Text = "Some content" },
					buttons
				}
			};
		}
	}
}
