using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;

namespace ${Namespace}
{	
	partial class ${EscapedIdentifier} : Form
	{	
		void InitializeComponent()
		{
			Title = "My ${EscapedIdentifier} form";

			Content = new StackLayout
			{
				Items =
				{
					new Label { Text = "Some Content" }
				}
			};
		}
	}
}
