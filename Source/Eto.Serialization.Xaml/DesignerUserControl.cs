using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Serialization.Xaml
{
	class DesignerUserControl : Panel
	{
		readonly Label label;
		public string Text
		{
			get { return label.Text; }
			set { label.Text = value; }
		}

		public string Tooltip
		{
			get { return label.ToolTip; }
			set { label.ToolTip = value; }
		}

		public DesignerUserControl()
		{
			BackgroundColor = Colors.White;
			Padding = new Padding(20);
			Content = label = new Label
			{
				VerticalAlignment = VerticalAlignment.Center,
				TextAlignment = TextAlignment.Center,
				Font = SystemFonts.Default(8),
				Text = "[User Control]"
			};
		}
	}
	
}