using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;

namespace ${Namespace}
{	
	partial class ${EscapedIdentifier} : Panel
	{	
		void InitializeComponent()
		{
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
