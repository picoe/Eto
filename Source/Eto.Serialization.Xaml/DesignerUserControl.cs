using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Eto.Drawing;

namespace Eto.Serialization.Xaml
{
	class DesignerUserControl : Eto.Forms.Panel
	{
		public DesignerUserControl()
		{
			BackgroundColor = Eto.Drawing.Colors.White;
			Padding = new Padding(20);
			Content = new Eto.Forms.Label
			{
				VerticalAlignment = Eto.Forms.VerticalAlignment.Center,
				TextAlignment = Eto.Forms.TextAlignment.Center,
				Font = SystemFonts.Default(8),
				Text = "[User Control]"
			};
		}
	}
	
}